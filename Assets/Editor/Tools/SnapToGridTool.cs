using UnityEditor;
using UnityEngine;

public class SnapToGridTool : EditorWindow
{
    private float m_GridSize = 1.0f;

    [MenuItem("Tools/SnapToGridTool")]
    public static void ShowWindow()
    {
        GetWindow<SnapToGridTool>("SnapToGridTool");
    }

    private void OnGUI()
    {
        m_GridSize = EditorGUILayout.FloatField("Grid Size", m_GridSize);

        if (GUILayout.Button("Snap Selected Objects"))
        {
            SnapSelectedObjects();
        }
    }

    private void SnapSelectedObjects()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Snap to Grid");
            Vector3 pos = obj.transform.position;

            pos.x = Mathf.Round(pos.x / m_GridSize) * m_GridSize;
            pos.y = 0; // Y always zero for now
            pos.z = Mathf.Round(pos.z / m_GridSize) * m_GridSize;

            obj.transform.position = pos;
        }

        Debug.Log($"Snapped {Selection.gameObjects.Length} objects to the grid.");
    }
}
