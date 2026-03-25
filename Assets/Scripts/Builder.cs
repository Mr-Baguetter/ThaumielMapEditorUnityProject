using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Components;
using Assets.Scripts.Enums;
using Assets.Scripts.Yaml;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class Builder : MonoBehaviour
    {
        public void CompileData()
        {
            SetupOutput(out string directoryPath);

            YamlSchematic schematic = new()
            {
                FileName = name.Replace(' ', '_'),
                Rotation = transform.rotation.eulerAngles,
                Scale = transform.localScale,
                Objects = CompileObjects()
            };
            CompileAnimators(directoryPath, schematic);
            File.WriteAllText(Path.Combine(directoryPath, $"{name}.yml"), YamlParser.Serializer.Serialize(schematic));
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
                }

                YamlCustomObject customObject = new()
                {
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