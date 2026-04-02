using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Components;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.Extensions;
using System.Linq;

namespace Assets.Scripts.Converter
{
    [InitializeOnLoad]
    public class PMERConverter : EditorWindow
    {
        public enum PMERBlockType
        {
            Empty = 0,
            Primitive = 1,
            Light = 2,
            Pickup = 3,
            Workstation = 4,
            Schematic = 5,
            Teleport = 6,
            Locker = 7,
            Text = 8,
            Interactable = 9,
        }

        private static Dictionary<int, Transform> _instanceMap = new();

        [MenuItem("Thaumiel/Tools/PMER Converter")]
        public static void Open()
        {
            _instanceMap.Clear();
            string[] guids = AssetDatabase.FindAssets("t:BuilderPrefabRegistry");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Decompiler._registry = AssetDatabase.LoadAssetAtPath<BuilderPrefabRegistry>(path);
            }

            string jsonPath = EditorUtility.OpenFilePanel("Select Schematic", "", "json");
            if (string.IsNullOrEmpty(jsonPath))
                return;

            string json = File.ReadAllText(jsonPath);
            YamlSchematic tmeschematic = ConvertSchematic(JsonConvert.DeserializeObject<PMERSchematic>(json), Path.GetFileNameWithoutExtension(jsonPath));
            GameObject root = new(tmeschematic.FileName);
            root.transform.rotation = Quaternion.Euler(tmeschematic.Rotation);
            root.transform.localScale = tmeschematic.Scale;
            root.AddComponent<Builder>();

            foreach (YamlCustomObject obj in tmeschematic.Objects)
            {
                GameObject prefab = Decompiler.GetPrefabForObject(obj);
                if (prefab == null)
                {
                    Debug.LogWarning($"No prefab mapped for ObjectType '{obj.ObjectType}', skipping '{obj.Name}'.");
                    continue;
                }

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.name = obj.Name;

                if (instance.TryGetComponent(out ObjectBase block))
                {
                    block.Name = obj.Name;
                    block.Static = obj.IsStatic;
                    block.MovementSmoothing = obj.MovementSmoothing;
                    block.ObjectType = obj.ObjectType;
                    block.Properties = obj.Values;

                    block.Position = obj.Position;
                    block.Rotation = obj.Rotation;
                    block.Scale = obj.Scale;

                    block.Decompile(root.transform);
                }

                ObjectBase originalBlock = root.GetComponents<ObjectBase>().First(b => b.Name == obj.Name);
                _instanceMap[originalBlock.ObjectId] = instance.transform;
                instance.transform.SetParent(GetParentForBlock(originalBlock, root.transform));
                Undo.RegisterCreatedObjectUndo(instance, $"Convert {obj.Name}");
            }

            Selection.activeGameObject = root;
        }

        public static Transform GetParentForBlock(ObjectBase block, Transform root)
        {
            if (block.ParentId != 0 && _instanceMap.TryGetValue(block.ParentId, out Transform parent))
                return parent;

            return root;
        }

        /// <summary>
        /// Converts a PMER schematic into a TME schematic
        /// </summary>
        /// <param name="root">The PMER schematic root</param>
        /// <returns></returns>
        public static YamlSchematic ConvertSchematic(PMERSchematic root, string filename)
        {
            YamlSchematic tme = new()
            {
                FileName = filename,
                Rotation = Vector3.one,
                Scale = Vector3.one,
                Objects = new()
            };

            foreach (PMERBlock block in root.Blocks)
                tme.Objects.Add(ConvertBlock(block));

            return tme;
        }

        private static YamlCustomObject ConvertBlock(PMERBlock block)
        {
            YamlCustomObject obj = new()
            {
                Name = block.Name,
                Position = block.Position,
                Rotation = block.Rotation,
                Scale = block.Scale,
                IsStatic = block.Properties != null && block.Properties.TryGetValue("Static", out object s) && Convert.ToBoolean(s),
                MovementSmoothing = 60,
                ObjectType = MapBlockType(block.BlockType),
                Values = NormalizeProperties(block)
            };

            return obj;
        }

        private static ObjectType MapBlockType(PMERBlockType blockType)
        {
            ObjectType Warn()
            {
                Debug.LogWarning($"Unsupported BlockType {blockType} was found. Setting type to None.");
                return ObjectType.None;
            }

            return blockType switch
            {
                PMERBlockType.Primitive => ObjectType.Primitive,
                PMERBlockType.Light => ObjectType.Light,
                PMERBlockType.Pickup => ObjectType.Pickup,
                PMERBlockType.Workstation => ObjectType.Workstation,
                PMERBlockType.Schematic => ObjectType.Schematic,
                PMERBlockType.Teleport => ObjectType.Teleporter,
                PMERBlockType.Locker => ObjectType.Locker,
                PMERBlockType.Text => ObjectType.TextToy,
                PMERBlockType.Interactable => ObjectType.Interactable,
                PMERBlockType.Empty => ObjectType.GameObject,
                _ => Warn(),
            };
        }

        private static Dictionary<string, object> NormalizeProperties(PMERBlock block)
        {
            Dictionary<string, object> dict = block.Properties != null ? new Dictionary<string, object>(block.Properties) : new();

            switch (block.BlockType)
            {
                case PMERBlockType.Primitive:
                    if (dict.TryGetValue("PrimitiveType", out var pt))
                        dict["PrimitiveType"] = Convert.ToInt32(pt);

                    if (dict.TryGetValue("PrimitiveFlags", out var pf))
                    {
                        dict["PrimitiveFlags"] = Convert.ToByte(pf);
                    }
                    else
                        dict["PrimitiveFlags"] = PrimitiveFlags.Visible | PrimitiveFlags.Collidable;

                    if (dict.TryGetValue("Color", out var color))
                    {
                        if (TryParseHexColor(color.ToString(), out Color unityColor))
                        {
                            dict["Color"] = unityColor;
                        }
                        else
                            Debug.LogWarning($"Failed to parse color value: {color}");
                    }
                    break;

                case PMERBlockType.Light:
                    if (dict.TryGetValue("LightType", out var lighttype))
                        dict["LightType"] = (LightType)Convert.ToInt32(lighttype);

                    if (dict.TryGetValue("Color", out var lightcolor) && TryParseHexColor(lightcolor.ToString(), out Color unitylightColor))
                        dict["LightColor"] = unitylightColor;

                    if (dict.TryGetValue("Intensity", out var intensity))
                        dict["LightIntensity"] = Convert.ToSingle(intensity);

                    if (dict.TryGetValue("Range", out var range))
                        dict["LightRange"] = Convert.ToSingle(range);

                    if (dict.TryGetValue("Shape", out var shape))
                        dict["LightShape"] = (LightShape)Convert.ToInt32(shape);

                    if (dict.TryGetValue("SpotAngle", out var spotangle))
                        dict["SpotAngle"] = Convert.ToSingle(spotangle);

                    if (dict.TryGetValue("InnerSpotAngle", out var innerspotangle))
                        dict["InnerSpotAngle"] = Convert.ToSingle(innerspotangle);

                    if (dict.TryGetValue("ShadowStrength", out var shadowStrength))
                        dict["ShadowStrength"] = Convert.ToSingle(shadowStrength);

                    if (dict.TryGetValue("ShadowType", out var shadowtype))
                        dict["ShadowType"] = (LightShadows)Convert.ToInt32(shadowtype);
                    break;
    
                case PMERBlockType.Pickup:
                    if (dict.TryGetValue("ItemType", out var itemtype))
                        dict["ItemToSpawn"] = (ItemType)Convert.ToInt32(itemtype);

                    if (dict.TryGetValue("Chance", out var chance))
                        dict["SpawnPercentage"] = Convert.ToSingle(chance);

                    if (dict.TryGetValue("Uses", out var uses))
                        dict["MaxAmount"] = Convert.ToInt32(uses);
                    break;

                case PMERBlockType.Teleport:
                    if (dict.TryGetValue("Cooldown", out var cooldown))
                        dict["Cooldown"] = Convert.ToSingle(cooldown);

                    if (dict.TryGetValue("Id", out var id))
                        dict["Id"] = Guid.Parse(Convert.ToString(id));

                    if (dict.TryGetValue("TargetTeleporters", out var targets))
                    {
                        if (targets is object[] array)
                        {
                            List<string?> ids = array.Select(t => t.GetType().GetField("Id")?.GetValue(t) as string).Where(id => id != null).ToList();
                            if (Guid.TryParse(ids.First(), out var targetid))
                                dict["Target"] = targetid;
                        }
                    }

                    break;

                case PMERBlockType.Interactable:
                    if (dict.TryGetValue("ColliderShape", out var collidershape))
                        dict["Shape"] = (ColliderShape)Convert.ToInt32(collidershape);

                    if (dict.TryGetValue("InteractionDuration", out var duration))
                        dict["Duration"] = Convert.ToSingle(duration);

                    if (dict.TryGetValue("IsLocked", out var locked))
                        dict["Locked"] = Convert.ToSingle(locked);

                    break;

                case PMERBlockType.Text:
                    if (dict.TryGetValue("Text", out var text))
                        dict["TextFormat"] = Convert.ToString(text);

                    if (dict.TryGetValue("DisplaySize", out var displaysize))
                        dict["DisplaySize"] = ConvertExtensions.ToVector2(displaysize);
                        
                    break;
            }

            return dict;
        }

        private static bool TryParseHexColor(string hex, out Color color)
        {
            color = Color.white;
            hex = hex.TrimStart('#');
            try
            {
                if (hex.Length == 6)
                {
                    color = new Color(
                        Convert.ToInt32(hex.Substring(0, 2), 16) / 255f,
                        Convert.ToInt32(hex.Substring(2, 2), 16) / 255f,
                        Convert.ToInt32(hex.Substring(4, 2), 16) / 255f
                    );

                    return true;
                }

                if (hex.Length == 8)
                {
                    color = new Color(
                        Convert.ToInt32(hex.Substring(0, 2), 16) / 255f,
                        Convert.ToInt32(hex.Substring(2, 2), 16) / 255f,
                        Convert.ToInt32(hex.Substring(4, 2), 16) / 255f,
                        Convert.ToInt32(hex.Substring(6, 2), 16) / 255f
                    );

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception parsing hex color '{hex}': {ex.Message}");
            }

            return false;
        }
    }
}