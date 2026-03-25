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

    private static void CompileAll()
    {
        foreach (Builder schematic in FindObjectsByType<Builder>(FindObjectsSortMode.None))
            schematic.CompileData();
    }
}
