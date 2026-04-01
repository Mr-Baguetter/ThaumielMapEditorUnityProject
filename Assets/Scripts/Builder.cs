using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Components;
using Assets.Scripts.Enums;
using Assets.Scripts.Areas;
using Assets.Scripts.Yaml;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class Builder : MonoBehaviour
    {
        [field: SerializeField]
        public List<YamlLOD> LODSettings { get; set; } = new();

        private static readonly Color[] LodColors = new Color[]
        {
            Color.green,
            Color.yellow,
            Color.cyan,
            Color.magenta,
            Color.red,
            Color.white
        };

        private void OnDrawGizmosSelected()
        {
            if (LODSettings == null)
                return;

            for (int i = 0; i < LODSettings.Count; i++)
            {
                YamlLOD lod = LODSettings[i];
                if (lod == null)
                    continue;

                Color lodColor = LodColors[i % LodColors.Length];

                Gizmos.color = lodColor;
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawWireCube(Vector3.zero, lod.Bounds);

                if (lod.Primitives == null)
                    continue;

                Gizmos.matrix = Matrix4x4.identity;

                foreach (PrimitiveObject primitive in GetComponentsInChildren<PrimitiveObject>())
                {
                    if (!lod.Primitives.Contains(primitive.PrimitiveType))
                        continue;

                    DrawCulledX(primitive.transform, lodColor);
                }
            }
        }

        private static void DrawCulledX(Transform t, Color color)
        {
            Gizmos.color = color;

            Vector3 center = t.position;
            float size = Mathf.Max(t.lossyScale.x, t.lossyScale.y, t.lossyScale.z) * 0.3f;

            Vector3 right = t.right * size;
            Vector3 up = t.up * size;

            Gizmos.DrawLine(center - right - up, center + right + up);
            Gizmos.DrawLine(center + right - up, center - right + up);
        }
        
        public static event Action<Builder>? OnBuild;

        public void CompileData()
        {
            SetupOutput(out string directoryPath);

            YamlSchematic schematic = new()
            {
                RootObjectId = transform.gameObject.GetInstanceID(),
                FileName = name.Replace(' ', '_'),
                Rotation = transform.rotation.eulerAngles,
                Scale = transform.localScale,
                Objects = CompileObjects(),
                Areas = CompileAreas(),
                LOD = LODSettings
            };
            
            CompileAnimators(directoryPath, schematic);
            File.WriteAllText(Path.Combine(directoryPath, $"{name}.yml"), YamlParser.Serializer.Serialize(schematic));
            OnBuild?.Invoke(this);
        }

        public List<YamlCustomObject> CompileObjects()
        {
            List<YamlCustomObject> customObjects = new();
            foreach (ObjectBase block in GetComponentsInChildren<ObjectBase>())
            {
                block.Compile(transform);
                switch (block)
                {
                    case PrimitiveObject:
                        block.Type = ObjectType.Primitive;
                        break;

                    case DoorObject:
                        block.Type = ObjectType.Door;
                        break;

                    case CameraObject:
                        block.Type = ObjectType.Camera;
                        break;

                    case ClutterObject:
                        block.Type = ObjectType.Clutter;
                        break;

                    case TextToyObject:
                        block.Type = ObjectType.TextToy;
                        break;

                    case CapyBaraObject:
                        block.Type = ObjectType.Capybara;
                        break;

                    case LightObject:
                        block.Type = ObjectType.Light;
                        break;

                    case LockerObject:
                        block.Type = ObjectType.Locker;
                        break;

                    case WorkstationObject:
                        block.Type = ObjectType.Workstation;
                        break;

                    case InteractableObject:
                        block.Type = ObjectType.Interactable;
                        break;

                    case WaypointObject:
                        block.Type = ObjectType.Waypoint;
                        break;

                    case PickupObject:
                        block.Type = ObjectType.Pickup;
                        break;

                    case TargetObject:
                        block.Type = ObjectType.Target;
                        break;

                    case TeleporterObject:
                        block.Type = ObjectType.Teleporter;
                        break;
                }

                YamlCustomObject customObject = new()
                {
                    ObjectId = block.ObjectId,
                    ParentId = block.ParentId,
                    Name = block.Name,
                    Position = block.Position,
                    Rotation = block.Rotation,
                    Scale = block.Scale,
                    IsStatic = block.Static,
                    MovementSmoothing = block.MovementSmoothing,
                    ObjectType = block.Type,
                    Values = block.Properties
                };

                customObjects.Add(customObject);
            }

            return customObjects;
        }

        private List<YamlArea> CompileAreas()
        {
            List<YamlArea> areas = new();
            foreach (AreaBase area in GetComponentsInChildren<AreaBase>())
            {
                switch (area)
                {
                    case CullableArea:
                        area.Type = AreaType.CullingArea;
                        break;
                }

                YamlArea yaml = new()
                {
                    ObjectId = area.ObjectId,
                    ParentId = area.ParentId,
                    SchematicName = name.Replace(' ', '_'),
                    AreaType = area.Type,
                    Values = area.Properties
                };

                areas.Add(yaml);
            }

            return areas;
        }

        private void CompileAnimators(string directoryPath, YamlSchematic schematic)
        {
            foreach (Animator animator in GetComponentsInChildren<Animator>())
            {
                RuntimeAnimatorController controller = animator.runtimeAnimatorController;
                if (controller == null)
                    continue;

                string controllerPath = AssetDatabase.GetAssetPath(controller);
                if (string.IsNullOrEmpty(controllerPath))
                {
                    Debug.LogWarning($"Could not find asset path for controller '{controller.name}', skipping.");
                    continue;
                }

                List<string> assetPaths = new() { controllerPath };
                foreach (AnimationClip clip in controller.animationClips)
                {
                    string clipPath = AssetDatabase.GetAssetPath(clip);
                    if (!string.IsNullOrEmpty(clipPath) && !assetPaths.Contains(clipPath))
                        assetPaths.Add(clipPath);
                }

                AssetBundleBuild[] builds = new AssetBundleBuild[]
                {
                    new()
                    {
                        assetBundleName = controller.name,
                        assetNames = assetPaths.ToArray()
                    }
                };

                BuildPipeline.BuildAssetBundles(directoryPath, builds, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode, EditorUserBuildSettings.activeBuildTarget);
                schematic.ContainsAnimator = true;
                Debug.Log($"Built animator controller '{controller.name}' for schematic '{name}'.");
            }
        }
        
        private void SetupOutput(out string directoryPath)
        {
            string parentDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ThaumielMapEditor");
            directoryPath = Path.Combine(parentDirectoryPath, name);

            if (!Directory.Exists(parentDirectoryPath))
                Directory.CreateDirectory(parentDirectoryPath);

            if (Directory.Exists(directoryPath))
                DeleteDirectory(directoryPath);

            if (File.Exists($"{directoryPath}.zip"))
                File.Delete($"{directoryPath}.zip");

            Directory.CreateDirectory(directoryPath);
        }

        private static void DeleteDirectory(string path)
        {
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
                DeleteDirectory(dir);

            Directory.Delete(path, false);
        }
    }
}