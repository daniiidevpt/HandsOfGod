#if UNITY_EDITOR
using HOG.Resources;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourceManager))]
public class ResourceManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ResourceManager resourceManager = (ResourceManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Behavior Tree Debug", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Wood"))
        {
            resourceManager.AddResource(new ResourceCollectionInfo(ResourceType.Wood, 1));
        }

        if (GUILayout.Button("Add Rock"))
        {
            resourceManager.AddResource(new ResourceCollectionInfo(ResourceType.Rock, 1));
        }

        if (GUILayout.Button("Remove Wood"))
        {
            resourceManager.RemoveResource(ResourceType.Wood, 1);
        }

        if (GUILayout.Button("Remove Rock"))
        {
            resourceManager.RemoveResource(ResourceType.Rock, 1);
        }
    }
}
#endif
