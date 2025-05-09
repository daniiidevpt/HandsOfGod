using System.Collections.Generic;
using UnityEngine;

namespace HOG.Grid
{
    public static class AStarPathfinder
    {
        public static List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld, GridManager gridManager, bool includeExactStart = false, bool includeExactEnd = false)
        {
            //Debug.Log("===== PATHFIND REQUEST =====");

            GridNode startNode = gridManager.GetNodeFromWorld(startWorld);
            GridNode targetNode = gridManager.GetNodeFromWorld(targetWorld);

            if (startNode == null)
            {
                Debug.LogWarning("Start node is NULL.");
                return null;
            }

            if (targetNode == null)
            {
                Debug.LogWarning("Target node is NULL.");
                return null;
            }

            //Debug.Log($"Start Node: {startNode.m_GridPos}, Walkable: {startNode.m_Walkable}");
            //Debug.Log($"Target Node: {targetNode.m_GridPos}, Walkable: {targetNode.m_Walkable}");

            if (!startNode.m_Walkable)
            {
                Debug.LogWarning("Start node is NOT walkable. Searching for closest walkable neighbor...");

                GridNode[] neighbors = gridManager.GetNeighbors(startNode);
                GridNode fallback = null;
                float minDist = float.MaxValue;

                foreach (var node in neighbors)
                {
                    if (!node.m_Walkable) continue;

                    float dist = Vector3.Distance(startWorld, node.m_WorldPos);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        fallback = node;
                    }
                }

                if (fallback == null)
                {
                    Debug.LogError("No walkable neighbor found near start node.");
                    return null;
                }

                startNode = fallback;
                Debug.Log($"Fallback start node used at {startNode.m_GridPos}");
            }

            if (!targetNode.m_Walkable)
            {
                Debug.LogWarning("Target node is NOT walkable.");
                return null;
            }

            // Reset all nodes before starting
            foreach (GridNode node in gridManager.Grid)
            {
                node.m_GCost = int.MaxValue;
                node.m_HCost = 0;
                node.m_Parent = null;
            }

            startNode.m_GCost = 0;
            startNode.m_HCost = GetDistance(startNode, targetNode);
            startNode.m_Parent = null;

            List<GridNode> openSet = new List<GridNode>();
            HashSet<GridNode> closedSet = new HashSet<GridNode>();

            openSet.Add(startNode);
            //Debug.Log($"Added start node to openSet: {startNode.m_GridPos}");

            //int step = 0;

            while (openSet.Count > 0)
            {
                GridNode current = openSet[0];

                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < current.FCost ||
                        (openSet[i].FCost == current.FCost && openSet[i].m_HCost < current.m_HCost))
                    {
                        current = openSet[i];
                    }
                }

                openSet.Remove(current);
                closedSet.Add(current);

                //Debug.Log($"Step {step++}: Checking node {current.m_GridPos}");

                if (current == targetNode)
                {
                    //Debug.Log("Target node reached by reference equality.");
                    var path = RetracePath(startNode, targetNode);

                    // Add precise world target at the end if requested
                    if (includeExactEnd)
                        path.Add(targetWorld);

                    // Add precise world start at the beginning if requested
                    if (includeExactStart)
                        path.Insert(0, startWorld);

                    return path;
                }

                if (current.m_GridPos == targetNode.m_GridPos)
                {
                    //Debug.Log("Target node reached by grid position match.");
                    var path = RetracePath(startNode, targetNode);

                    // Add precise world target at the end if requested
                    if (includeExactEnd)
                        path.Add(targetWorld);

                    // Add precise world start at the beginning if requested
                    if (includeExactStart)
                        path.Insert(0, startWorld);

                    return path;
                }

                foreach (GridNode neighbor in gridManager.GetNeighbors(current))
                {
                    bool isTarget = neighbor == targetNode;

                    if (!neighbor.m_Walkable || (neighbor.m_IsPathBlocked && !isTarget) || closedSet.Contains(neighbor))
                        continue;

                    if (neighbor.m_GridPos == targetNode.m_GridPos)
                    {
                        //Debug.Log($"Target node seen as neighbor: {neighbor.m_GridPos}, walkable = {neighbor.m_Walkable}");
                    }

                    int newGCost = current.m_GCost + GetDistance(current, neighbor);
                    if (newGCost < neighbor.m_GCost || !openSet.Contains(neighbor))
                    {
                        neighbor.m_GCost = newGCost;
                        neighbor.m_HCost = GetDistance(neighbor, targetNode);
                        neighbor.m_Parent = current;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                            //Debug.Log($"Added neighbor {neighbor.m_GridPos} to openSet");
                        }
                    }
                }
            }

            Debug.LogWarning("No path could be found.");
            return null;
        }

        private static List<Vector3> RetracePath(GridNode startNode, GridNode endNode, Vector3? overrideFinal = null)
        {
            List<Vector3> path = new List<Vector3>();
            GridNode currentNode = endNode;

            while (currentNode != null && currentNode != startNode)
            {
                path.Add(currentNode.m_WorldPos);
                currentNode = currentNode.m_Parent;
            }

            path.Reverse();

            if (overrideFinal.HasValue)
                path.Add(overrideFinal.Value);

            return path;
        }

        private static int GetDistance(GridNode a, GridNode b)
        {
            int dx = Mathf.Abs(a.m_GridPos.x - b.m_GridPos.x);
            int dy = Mathf.Abs(a.m_GridPos.y - b.m_GridPos.y);

            if (dx > dy)
                return 14 * dy + 10 * (dx - dy);

            return 14 * dx + 10 * (dy - dx);
        }
    }
}
