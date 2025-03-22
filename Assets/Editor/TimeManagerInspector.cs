using HOG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimeManager))]
public class TimeManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TimeManager manager = (TimeManager)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Time", EditorStyles.boldLabel);

        string timeOfDay = manager.GetTimeOfDay(manager.CurrentTime);
        EditorGUILayout.LabelField($"Time: {Mathf.Floor(manager.CurrentTime)}:00 - {timeOfDay}");

        if (Application.isPlaying)
        {
            EditorUtility.SetDirty(target); // Refresh inspector in play mode
        }
    }
}
