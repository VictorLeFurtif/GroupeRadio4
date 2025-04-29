using UnityEngine;
using UnityEditor;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts")]
    public static void ShowWindow()
    {
        int go_count = 0, components_count = 0, missing_count = 0;
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            if (go.hideFlags == HideFlags.None)
            {
                go_count++;
                Component[] components = go.GetComponents<Component>();

                for (int i = 0; i < components.Length; i++)
                {
                    components_count++;
                    if (components[i] == null)
                    {
                        missing_count++;
                        Debug.LogWarning($"Missing script in GameObject: '{GetFullPath(go)}'", go);
                    }
                }
            }
        }
        Debug.Log($"Searched {go_count} GameObjects, {components_count} components, found {missing_count} missing");
    }

    private static string GetFullPath(GameObject go)
    {
        string path = go.name;
        while (go.transform.parent != null)
        {
            go = go.transform.parent.gameObject;
            path = go.name + "/" + path;
        }
        return path;
    }
}