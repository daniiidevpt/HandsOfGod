using UnityEngine;

namespace HOG.Grid
{
    public class GridNode
    {
        public Vector2Int m_GridPos;
        public Vector3 m_WorldPos;
        public bool m_Walkable;
        public bool m_IsPathBlocked;

        public int m_GCost;
        public int m_HCost;
        public int FCost => m_GCost + m_HCost;

        public GridNode m_Parent;

        public GridNode(Vector2Int gridPos, Vector3 worldPos, bool walkable)
        {
            m_GridPos = gridPos;
            m_WorldPos = worldPos;
            m_Walkable = walkable;
            m_IsPathBlocked = false;
        }
    }
}
