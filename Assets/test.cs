using HOG.Grid;
using UnityEngine;

public class test : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 targetPos;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var path = AStarPathfinder.FindPath(startPos, targetPos, GridManager.Instance, true, true);

            GridNode startNode = GridManager.Instance.GetNodeFromWorld(startPos);
            GridNode endNode = GridManager.Instance.GetNodeFromWorld(targetPos);

            Debug.Log($"Start Node: {(startNode != null ? startNode.m_GridPos.ToString() : "null")}");
            Debug.Log($"End Node: {(endNode != null ? endNode.m_GridPos.ToString() : "null")}");

            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("No path found!");
            }
            else
            {
                Debug.Log($"Path found with {path.Count} nodes.");
                GridManager.Instance.DebugVisualizer.SetPath(GetInstanceID().ToString(),path);
            }

            //Vector3 worldPos = new Vector3(3, 0, -2);
            //var node = GridManager.Instance.GetNodeFromWorld(worldPos);

            //Debug.Log($"Transform: {worldPos} → GridNode: {node?.m_GridPos} → NodeWorldPos: {node?.m_WorldPos}");
        }
    }
}
