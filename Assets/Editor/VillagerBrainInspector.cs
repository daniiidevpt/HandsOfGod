#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using HOG.Villager;

[CustomEditor(typeof(VillagerBrain))]
public class VillagerBrainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //VillagerBrain brain = (VillagerBrain)target;

        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField("Behavior Tree Debug", EditorStyles.boldLabel);

        //if (brain.CurrentNode != null)
        //{
        //    EditorGUILayout.LabelField("Current Running Node:", brain.CurrentNode.Name, EditorStyles.helpBox);
        //}
        //else
        //{
        //    EditorGUILayout.LabelField("Current Running Node:", "None (Waiting for Evaluation)", EditorStyles.helpBox);
        //}

        if (Application.isPlaying)
        {
            EditorUtility.SetDirty(target); // Refresh inspector in play mode
        }
    }
}
#endif