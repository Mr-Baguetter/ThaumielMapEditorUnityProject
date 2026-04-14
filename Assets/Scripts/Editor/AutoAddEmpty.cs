using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Components;

namespace Assets.Scripts.Editor
{
    // Todo: Test
    [InitializeOnLoad]
    public class AutoAddEmpty
    {
        private static HashSet<int> existingObjects = new();

        static AutoAddEmpty()
        {
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (GameObject go in allObjects)
            {
                existingObjects.Add(go.GetInstanceID());
            }

            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (GameObject go in allObjects)
            {
                if (!existingObjects.Contains(go.GetInstanceID()))
                {
                    existingObjects.Add(go.GetInstanceID());
                    if (go.GetComponent<ObjectBase>() == null && go.GetComponent<Builder>() == null)
                    {
                        go.AddComponent<EmptyGameObject>();
                        Debug.Log($"Automatically added EmptyGameObject to {go.name}");
                    }
                }
            }
        }
    }
}
