using UnityEditor;
using UnityEngine;
using System.IO;
using System;

namespace Assets.Scripts.Editor
{
    public class ConfigEditorWindow : EditorWindow
    {
        private class PMERConfig
        {
            public bool OpenDirectoryAfterCompiling = false;
            public string ExportPath = string.Empty;
            public bool ZipCompiledSchematics = false;
        }

        private Config current;

        [MenuItem("SchematicManager/Builder Config")]
        public static void ShowWindow()
        {
            ConfigEditorWindow window = GetWindow<ConfigEditorWindow>("Builder Config");
            window.minSize = new Vector2(400, 150);
            window.Show();
        }

        private void OnEnable()
        {
            current = ConfigBuilder.LoadConfig();
        }

        private void OnGUI()
        {
            current ??= ConfigBuilder.LoadConfig();

            GUILayout.Space(10);
            GUILayout.Label("Builder Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();

            current.ExportPath = EditorGUILayout.TextField("Export Path", current.ExportPath);
            if (GUILayout.Button("Browse", GUILayout.Width(75)))
            {
                string defaultPath = string.IsNullOrEmpty(current.ExportPath) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ThaumielMapEditor") : current.ExportPath;
                string selectedPath = EditorUtility.OpenFolderPanel("Select Export Directory", defaultPath, "");
                
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    GUI.FocusControl(null); 
                    current.ExportPath = selectedPath;
                }
            }

            if (GUILayout.Button("Convert PMER Settings", GUILayout.Height(30)))
            {
                ConvertPMERSettings();
            }

            GUILayout.EndHorizontal();
            current.CompressExport = EditorGUILayout.Toggle("Compress Export", current.CompressExport);
            current.OpenExportAfterCompiling = EditorGUILayout.Toggle("Open Export", current.OpenExportAfterCompiling);

            if (EditorGUI.EndChangeCheck())
                ConfigBuilder.SaveConfig(current);

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Force Save Config", GUILayout.Height(30)))
            {
                ConfigBuilder.SaveConfig(current);
                Debug.Log("Builder Config saved successfully!");
            }
            
            GUILayout.Space(10);
        }

        private void ConvertPMERSettings()
        {
            string path = EditorUtility.OpenFilePanel("Select PMER Config", "", "json");
            if (string.IsNullOrEmpty(path))
                return;

            try 
            {
                PMERConfig config = JsonUtility.FromJson<PMERConfig>(File.ReadAllText(path));
                if (config != null)
                {
                    current.CompressExport = config.ZipCompiledSchematics;
                    current.ExportPath = config.ExportPath;
                    current.OpenExportAfterCompiling = config.OpenDirectoryAfterCompiling;
                    
                    ConfigBuilder.SaveConfig(current);
                    GUI.FocusControl(null); 
                    Debug.Log("PMER settings imported and saved successfully!");
                }
                else
                    Debug.LogWarning("Selected file was not a valid PMER Config or was empty.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse PMER config. Please ensure it's a valid JSON file. Error: {ex.Message}");
            }
        }
    }
}