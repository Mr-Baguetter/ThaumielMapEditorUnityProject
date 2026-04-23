using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Components;

namespace Assets.Scripts
{
#pragma warning disable CS0618
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
                if (go.TryGetComponent<EmptyGameObject>(out var empty))
                {
                    Component[] components = go.GetComponents<Component>();
                    bool hasOtherComponent = false;

                    foreach (Component c in components)
                    {
                        if (c is Transform || c is EmptyGameObject)
                            continue;

                        hasOtherComponent = true;
                        break;
                    }

                    if (hasOtherComponent)
                    {
                        Object.DestroyImmediate(empty);
                        Debug.Log($"Removed EmptyGameObject from {go.name} because another component was added.");
                        continue;
                    }
                }

                if (!existingObjects.Contains(go.GetInstanceID()))
                {
                    existingObjects.Add(go.GetInstanceID());
                    if (go.GetComponent<ObjectBase>() == null && go.GetComponent<Builder>() == null && go.GetComponentInParent<ObjectBase>() == null && go.GetComponent<ServerSide>() == null && go.name.Contains("GameObject"))
                    {
                        go.AddComponent<EmptyGameObject>();
                        Debug.Log($"Automatically added EmptyGameObject to {go.name}");
                    }
                }
            }
        }
    }
}
