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
    public static class Decompiler
    {
        private static BuilderPrefabRegistry _registry;

        public static void DecompileData(BuilderPrefabRegistry registry)
        {
            _registry = registry;

            string yamlPath = EditorUtility.OpenFilePanel("Select Schematic", "", "yml");
            if (string.IsNullOrEmpty(yamlPath))
                return;

            string yaml = File.ReadAllText(yamlPath);
            YamlSchematic schematic = YamlParser.Deserializer.Deserialize<YamlSchematic>(yaml);

            GameObject root = new(schematic.FileName);
            root.transform.rotation = Quaternion.Euler(schematic.Rotation);
            root.transform.localScale = schematic.Scale;

            Undo.RegisterCreatedObjectUndo(root, $"Decompile {schematic.FileName}");

            foreach (YamlCustomObject obj in schematic.Objects)
            {
                GameObject prefab = GetPrefabForObject(obj);
                if (prefab == null)
                {
                    Debug.LogWarning($"No prefab mapped for ObjectType '{obj.ObjectType}', skipping '{obj.Name}'.");
                    continue;
                }

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.transform);
                instance.AddComponent<Builder>();
                instance.name = obj.Name;

                if (instance.TryGetComponent(out ObjectBase block))
                {
                    block.Name = obj.Name;
                    block.Static = obj.IsStatic;
                    block.MovementSmoothing = obj.MovementSmoothing;
                    block.Type = obj.ObjectType;
                    block.Properties = obj.Values;

                    block.Position = obj.Position;
                    block.Rotation = obj.Rotation;
                    block.Scale = obj.Scale;

                    block.Decompile(root.transform);
                }

                Undo.RegisterCreatedObjectUndo(instance, $"Decompile {obj.Name}");
            }

            Selection.activeGameObject = root;
            Debug.Log($"Decompiled schematic '{schematic.FileName}' with {schematic.Objects.Count} objects.");
        }

        private static void LoadRegistry()
        {
            string[] guids = AssetDatabase.FindAssets("t:BuilderPrefabRegistry");
            if (guids.Length == 0)
                return;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _registry = AssetDatabase.LoadAssetAtPath<BuilderPrefabRegistry>(path);
        }

        private static GameObject GetPrefabForObject(YamlCustomObject obj)
        {
            return obj.ObjectType switch
            {
                ObjectType.Primitive => GetPrimitivePrefab(obj),
                ObjectType.Door => GetDoorPrefab(obj),
                ObjectType.Camera => GetCameraPrefab(obj),
                ObjectType.Clutter => GetClutterPrefab(obj),
                ObjectType.Locker => GetLockerPrefab(obj),
                ObjectType.Target => GetTargetPrefab(obj),
                ObjectType.TextToy => _registry.TextToyPrefab,
                ObjectType.Capybara => _registry.CapybaraPrefab,
                ObjectType.Light => _registry.LightPrefab,
                ObjectType.Workstation => _registry.WorkstationPrefab,
                ObjectType.Interactable => _registry.InteractablePrefab,
                ObjectType.Waypoint => _registry.WaypointPrefab,
                ObjectType.Pickup => _registry.PickupPrefab,
                _ => null
            };
        }

        private static GameObject GetPrimitivePrefab(YamlCustomObject obj)
        {
            if (!obj.Values.TryGetValue("PrimitiveType", out object primitiveType))
                return null;

            return Enum.Parse<PrimitiveType>(primitiveType.ToString()) switch
            {
                PrimitiveType.Sphere => _registry.SpherePrefab,
                PrimitiveType.Cube => _registry.CubePrefab,
                PrimitiveType.Cylinder => _registry.CylinderPrefab,
                PrimitiveType.Capsule => _registry.CapsulePrefab,
                PrimitiveType.Plane => _registry.PlanePrefab,
                PrimitiveType.Quad => _registry.QuadPrefab,
                _ => null
            };
        }

        private static GameObject GetDoorPrefab(YamlCustomObject obj)
        {
            if (!obj.Values.TryGetValue("DoorType", out object doorType))
                return null;

            return Enum.Parse<DoorType>(doorType.ToString()) switch
            {
                DoorType.Lcz => _registry.LczDoorPrefab,
                DoorType.Hcz => _registry.HczDoorPrefab,
                DoorType.Ez => _registry.EzDoorPrefab,
                DoorType.Gate => _registry.GateDoorPrefab,
                DoorType.BulkHead => _registry.BulkHeadDoorPrefab,
                _ => null
            };
        }

        private static GameObject GetCameraPrefab(YamlCustomObject obj)
        {
            if (!obj.Values.TryGetValue("CameraType", out object cameraType))
                return null;

            return Enum.Parse<Enums.CameraType>(cameraType.ToString()) switch
            {
                Enums.CameraType.Lcz => _registry.LczCameraPrefab,
                Enums.CameraType.Hcz => _registry.HczCameraPrefab,
                Enums.CameraType.Ez => _registry.EzCameraPrefab,
                Enums.CameraType.EzArm => _registry.EzArmCameraPrefab,
                Enums.CameraType.Sz => _registry.SzCameraPrefab,
                _ => null
            };
        }

        private static GameObject GetClutterPrefab(YamlCustomObject obj)
        {
            if (!obj.Values.TryGetValue("ClutterType", out object clutterType))
                return null;

            return Enum.Parse<ClutterType>(clutterType.ToString()) switch
            {
                ClutterType.SimpleBoxes => _registry.SimpleBoxesPrefab,
                ClutterType.PipesShort => _registry.PipesShortPrefab,
                ClutterType.BoxesLadder => _registry.BoxesLadderPrefab,
                ClutterType.TankSupportedShelf => _registry.TankSupportedShelfPrefab,
                ClutterType.AngledFences => _registry.AngledFencesPrefab,
                ClutterType.HugeOrangePipes => _registry.HugeOrangePipesPrefab,
                ClutterType.PipesLongOpen => _registry.PipesLongOpenPrefab,
                ClutterType.BrokenElectricalBox => _registry.BrokenElectricalBoxPrefab,
                _ => null
            };
        }

        private static GameObject GetLockerPrefab(YamlCustomObject obj)
        {
            if (!obj.Values.TryGetValue("LockerType", out object lockerType))
                return null;

            return Enum.Parse<LockerType>(lockerType.ToString()) switch
            {
                LockerType.Pedestal => _registry.PedestalPrefab,
                LockerType.LargeGun => _registry.LargeGunPrefab,
                LockerType.RifleRack => _registry.RifleRackPrefab,
                LockerType.Misc => _registry.MiscLockerPrefab,
                LockerType.Medkit => _registry.MedkitPrefab,
                LockerType.Adrenaline => _registry.AdrenalinePrefab,
                LockerType.ExperimentalWeapon => _registry.ExperimentalWeaponPrefab,
                _ => null
            };
        }

        private static GameObject GetTargetPrefab(YamlCustomObject obj)
        {
            if (!obj.Values.TryGetValue("TargetType", out object targetType))
                return null;

            return Enum.Parse<TargetType>(targetType.ToString()) switch
            {
                TargetType.Binary => _registry.BinaryTargetPrefab,
                TargetType.ClassD => _registry.ClassDTargetPrefab,
                TargetType.Sport => _registry.SportTargetPrefab,
                _ => null
            };
        }
    }
}