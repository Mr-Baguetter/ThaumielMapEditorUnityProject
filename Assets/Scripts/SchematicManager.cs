using Assets.Scripts;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SchematicManager : EditorWindow
{
    [MenuItem("SchematicManager/Compile %#d")]
    public static void Compile()
    {
        Debug.ClearDeveloperConsole();
        CompileAll();
    }

    [MenuItem("SchematicManager/Decompile Schematics %#e")]
    public static void Decompile()
    {
        string[] guids = AssetDatabase.FindAssets("t:BuilderPrefabRegistry");
        if (guids.Length == 0)
        {
            Debug.LogError("No BuilderPrefabRegistry found. Create one via Thaumiel/Decompiler.");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        BuilderPrefabRegistry registry = AssetDatabase.LoadAssetAtPath<BuilderPrefabRegistry>(path);
        Decompiler.DecompileData(registry);
    }

    private static void CompileAll()
    {
        foreach (Builder schematic in FindObjectsByType<Builder>(FindObjectsSortMode.None))
            schematic.CompileData();
    }
}
