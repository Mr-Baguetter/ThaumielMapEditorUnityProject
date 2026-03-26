using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    public class DecompilerWindow : EditorWindow
    {
        private BuilderPrefabRegistry _registry;

        [MenuItem("Thaumiel/Decompiler")]
        public static void Open()
        {
            GetWindow<DecompilerWindow>("Decompiler");
        }

        private void OnEnable()
        {
            string[] guids = AssetDatabase.FindAssets("t:BuilderPrefabRegistry");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _registry = AssetDatabase.LoadAssetAtPath<BuilderPrefabRegistry>(path);
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Prefab Registry", EditorStyles.boldLabel);

            _registry = (BuilderPrefabRegistry)EditorGUILayout.ObjectField(
                "Registry", _registry, typeof(BuilderPrefabRegistry), false);

            if (_registry == null)
            {
                EditorGUILayout.HelpBox("Assign or create a Prefab Registry asset.", MessageType.Warning);

                if (GUILayout.Button("Create Registry"))
                    CreateRegistry();

                return;
            }

            EditorGUI.BeginChangeCheck();

            SerializedObject serialized = new(_registry);
            serialized.Update();

            EditorGUI.BeginChangeCheck();

            SerializedProperty prop = serialized.GetIterator();
            prop.NextVisible(true);

            while (prop.NextVisible(false))
                EditorGUILayout.PropertyField(prop, true);

            if (EditorGUI.EndChangeCheck())
                serialized.ApplyModifiedProperties();

            GUILayout.Space(10);

            if (GUILayout.Button("Decompile Schematic"))
                DecompileSchematic();
        }

        private void DecompileSchematic()
        {
            if (_registry == null)
            {
                Debug.LogError("No Prefab Registry assigned.");
                return;
            }

            Decompiler.DecompileData(_registry);
        }

        private void CreateRegistry()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Prefab Registry", "BuilderPrefabRegistry", "asset", "Choose where to save the registry.");

            if (string.IsNullOrEmpty(path))
                return;

            BuilderPrefabRegistry registry = CreateInstance<BuilderPrefabRegistry>();
            AssetDatabase.CreateAsset(registry, path);
            AssetDatabase.SaveAssets();
            _registry = registry;
        }
    }
}