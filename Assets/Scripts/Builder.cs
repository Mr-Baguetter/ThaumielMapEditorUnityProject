using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Components;
using Assets.Scripts.Components.Tools;
using Assets.Scripts.Yaml;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
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

        private static readonly Color CullingAreaColor = new Color(1f, 0.5f, 0f);

        [SerializeField]
        internal ServerSide server;

        private void Awake()
        {
            server = GetComponentInChildren<ServerSide>();
            if (server != null)
                return;

            GameObject obj = new("ServerSideObjects");
            obj.transform.SetParent(transform);
            server = obj.AddComponent<ServerSide>();
        }

        private void LateUpdate()
        {
            if (server == null)
                return;

            server.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }

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
        
        public static event Action<Builder>? OnBuilt;

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
                ServerSideObjects = CompileServerObjects(directoryPath),
                Areas = new(),
                LOD = LODSettings
            };
            
            File.WriteAllText(Path.Combine(directoryPath, $"{name}.yml"), YamlParser.Serializer.Serialize(schematic));
            OnBuilt?.Invoke(this);
        }

        public List<YamlCustomObject> CompileServerObjects(string directoryPath)
        {
            List<YamlCustomObject> serverObjects = new();
            
            GameObject serverobj = GetComponentsInChildren<ServerSide>().First().gameObject;
            foreach (ObjectBase block in serverobj.GetComponentsInChildren<ObjectBase>())
            {
                block.ServerSide = true;
                block.Compile(transform);
                if (block.TryGetComponent(out Animator animator) && animator.runtimeAnimatorController != null)
                {
                    RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
                    AssetBundleBuild bundleBuild = new()
                    {
                        assetBundleName = runtimeAnimatorController.name,
                        assetNames = new[] { AssetDatabase.GetAssetPath(runtimeAnimatorController) }
                    };

                    BuildPipeline.BuildAssetBundles(directoryPath, new[] { bundleBuild }, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.StrictMode, EditorUserBuildSettings.activeBuildTarget);
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
                    ObjectType = block.ObjectType,
                    Values = block.Properties,
                    AnimatorName = block.gameObject.GetComponent<Animator>().name ?? string.Empty,
                    Tools = CompileTools(block)
                };

                serverObjects.Add(customObject);
            }

            return serverObjects;
        }

        public List<YamlTool> CompileTools(ObjectBase block)
        {
            List<YamlTool> tools = new();

            foreach (ToolBase toolBase in block.GetComponents<ToolBase>())
            {
                toolBase.Compile();
                YamlTool tool = new()
                {
                    ToolName = toolBase.ToolType.ToString(),
                    Properties = toolBase.Properties
                };
                
                tools.Add(tool);
            }

            return tools;
        }

        public List<YamlCustomObject> CompileObjects()
        {
            List<YamlCustomObject> customObjects = new();

            ServerSide serverSide = GetComponentInChildren<ServerSide>();

            foreach (ObjectBase block in GetComponentsInChildren<ObjectBase>())
            {
                if (serverSide != null && block.transform.IsChildOf(serverSide.transform))
                    continue;

                block.Compile(transform);
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
                    ObjectType = block.ObjectType,
                    Values = block.Properties
                };

                customObjects.Add(customObject);
            }

            return customObjects;
        }

        // TODO: Test this.
        /*
        private List<YamlArea> CompileCulling()
        {
            List<YamlArea> areas = new();
            CullingSettings.Compile(transform);
            YamlArea culling = new()
            {
                ObjectId = transform.gameObject.GetInstanceID(),
                ParentId = transform.gameObject.GetInstanceID(),
                SchematicName = name.Replace(' ', '_'),
                AreaType = AreaType.CullingArea,
                Values = CullingSettings.Properties
            };

            areas.Add(culling);
            return areas;
        }
        */

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