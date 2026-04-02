using UnityEditor;
using UnityEngine;

public class RemoveMissingScripts : EditorWindow
{
    private GameObject _prefab;

    [MenuItem("Thaumiel/Tools/Remove Missing Scripts")]
    public static void Open()
    {
        GetWindow<RemoveMissingScripts>("Remove Missing Scripts");
    }

    private void OnGUI()
    {
        GUILayout.Label("Drag & drop a prefab to remove missing scripts from it.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.Space();

        _prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", _prefab, typeof(GameObject), false);

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(_prefab == null);
        if (GUILayout.Button("Remove Missing Scripts"))
            RemoveMissing();
            
        EditorGUI.EndDisabledGroup();
    }

    private void RemoveMissing()
    {
        string path = AssetDatabase.GetAssetPath(_prefab);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning($"{_prefab.name} is not a project asset.");
            return;
        }

        int totalRemoved = 0;
        foreach (Transform t in _prefab.GetComponentsInChildren<Transform>(true))
        {
            totalRemoved += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(t.gameObject);
        }

        if (totalRemoved > 0)
        {
            PrefabUtility.SavePrefabAsset(_prefab);
            AssetDatabase.SaveAssets();
            Debug.Log($"Removed {totalRemoved} missing scripts from {path}");
        }
        else
            Debug.Log($"No missing scripts found on {_prefab.name}");
    }
}