using UnityEditor;
using UnityEngine;
using HOG.Grid;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridManager gridManager = (GridManager)target;

        GUILayout.Space(10);
        GUILayout.Label("Editor Tools", EditorStyles.boldLabel);

        if (!Application.isPlaying)
        {
            if (GUILayout.Button(gridManager.IsEditorGridGenerated ? "Hide Grid" : "Show Grid"))
            {
                if (gridManager.IsEditorGridGenerated)
                {
                    gridManager.ClearGrid();
                }
                else
                {
                    gridManager.GenerateGrid(gridManager.GridOrigin);
                }

                EditorUtility.SetDirty(gridManager);
                SceneView.RepaintAll();
            }
        }

        if (GUILayout.Button(gridManager.ShowCoordinates ? "Hide Coordinates" : "Show Coordinates"))
        {
            gridManager.ShowCoordinates = !gridManager.ShowCoordinates;
            EditorUtility.SetDirty(gridManager);
        }


        if (GUILayout.Button("Center Origin on World (0,0)"))
        {
            gridManager.GridOrigin = Vector3.zero;
            gridManager.GenerateGrid(Vector3.zero);
            EditorUtility.SetDirty(gridManager);
        }
    }
}
