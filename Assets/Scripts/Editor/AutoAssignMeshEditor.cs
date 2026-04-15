using UnityEngine;
using UnityEditor;

public class AutoAssignMeshEditor : Editor
{
    [MenuItem("Thaumiel/Tools/Assign & Save Mesh")]
    static void AssignMesh()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            Debug.LogError("No GameObject selected!");
            return;
        }

        foreach (SkinnedMeshRenderer smr in selected.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (smr == null)
            {
                Debug.LogError("No SkinnedMeshRenderer found on selected GameObject!");
                continue;
            }

            Mesh mesh = Resources.Load<Mesh>("Mesh/References/" + smr.transform.name);

            if (mesh != null)
            {
                Undo.RecordObject(smr, "Assign Mesh");
                smr.sharedMesh = mesh;
                EditorUtility.SetDirty(smr);
                AssetDatabase.SaveAssets();
                Debug.Log($"Mesh '{mesh.name}' assigned and saved!");
            }
            else
            {
                Debug.LogError($"No mesh found for: '{selected.name}' in Assets/Resources/Mesh/References/");
            }
        }

        foreach (MeshFilter smr in selected.GetComponentsInChildren<MeshFilter>())
        {
            if (smr == null)
            {
                Debug.LogError("No MeshFilter found on selected GameObject!");
                continue;
            }

            Mesh mesh = Resources.Load<Mesh>("Mesh/References/" + smr.transform.name);

            if (mesh != null)
            {
                Undo.RecordObject(smr, "Assign Mesh");
                smr.sharedMesh = mesh;
                EditorUtility.SetDirty(smr);
                AssetDatabase.SaveAssets();
                Debug.Log($"Mesh '{mesh.name}' assigned and saved!");
            }
            else
            {
                Debug.LogError($"No mesh found for: '{selected.name}' in Assets/Resources/Mesh/References/");
            }
        }
    }
}