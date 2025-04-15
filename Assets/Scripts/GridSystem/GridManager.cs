using System.Collections.Generic;
using UnityEngine;

namespace HOG.Grid
{
    [ExecuteInEditMode]
    public class GridManager : Singleton<GridManager>
    {
        [Header("Grid Settings")]
        [SerializeField] private Vector2Int m_GridSize = new Vector2Int(20, 20);
        [SerializeField] private float m_CellSize = 1f;
        [SerializeField] private LayerMask m_UnwalkableMask;
        [SerializeField] private LayerMask m_PathBlockedMask;
        [SerializeField] private bool m_AllowDiagonals = true;
        [SerializeField] private Vector3 m_GridOrigin;
        public Vector3 GridOrigin { get => m_GridOrigin; set => m_GridOrigin = value; }

        [Header("Debug Settings")]
        [SerializeField] private bool m_EnableDebugDraw = true;
        [SerializeField] private bool m_ShowCoordinates = false;
        [SerializeField] private Color m_WalkableColor = Color.white;
        [SerializeField] private Color m_UnwalkableColor = Color.red;
        [SerializeField] private Color m_PathBlockedColor = Color.yellow;
        public bool ShowCoordinates { get => m_ShowCoordinates; set => m_ShowCoordinates = value; }

        private PathDebugVisualizer m_DebugVisualizer;
        public PathDebugVisualizer DebugVisualizer => m_DebugVisualizer;


        private GridNode[,] m_Grid;
        private Vector2Int m_GridOffset;

        public GridNode[,] Grid => m_Grid;

#if UNITY_EDITOR
        [HideInInspector] private bool m_IsEditorGridGenerated = false;
        public bool IsEditorGridGenerated => m_IsEditorGridGenerated;
#endif

        protected override void Awake()
        {
            base.Awake();

            m_DebugVisualizer = GetComponent<PathDebugVisualizer>();

            GenerateGrid(m_GridOrigin);
        }

        public void GenerateGrid(Vector3 origin)
        {
#if UNITY_EDITOR
            m_IsEditorGridGenerated = true;
#endif

            m_Grid = new GridNode[m_GridSize.x, m_GridSize.y];
            m_GridOffset = new Vector2Int(m_GridSize.x / 2, m_GridSize.y / 2);

            for (int x = 0; x < m_GridSize.x; x++)
            {
                for (int y = 0; y < m_GridSize.y; y++)
                {
                    int gridX = x - m_GridOffset.x;
                    int gridY = y - m_GridOffset.y;
                    Vector2Int gridPos = new Vector2Int(gridX, gridY);

                    Vector3 worldPos = origin + new Vector3(gridX * m_CellSize, 0, gridY * m_CellSize);

                    bool walkable = !Physics.CheckBox(worldPos, Vector3.one * m_CellSize * 0.4f, Quaternion.identity, m_UnwalkableMask);
                    GridNode node = new GridNode(gridPos, worldPos, walkable);

                    if (walkable && Physics.CheckBox(worldPos, Vector3.one * m_CellSize * 0.4f, Quaternion.identity, m_PathBlockedMask))
                    {
                        node.m_IsPathBlocked = true;
                    }

                    m_Grid[x, y] = node;
                }
            }

            //Debug.Log($"Grid generated from origin: {m_GridOrigin}, size: {m_GridSize}, cell size: {m_CellSize}");
        }

        public void ClearGrid()
        {
            m_Grid = null;
            m_IsEditorGridGenerated = false;
        }

        public GridNode GetNodeFromWorld(Vector3 worldPosition)
        {
            int gridX = Mathf.RoundToInt((worldPosition.x - m_GridOrigin.x) / m_CellSize);
            int gridY = Mathf.RoundToInt((worldPosition.z - m_GridOrigin.z) / m_CellSize);

            int arrayX = gridX + m_GridOffset.x;
            int arrayY = gridY + m_GridOffset.y;

            if (arrayX < 0 || arrayX >= m_GridSize.x || arrayY < 0 || arrayY >= m_GridSize.y)
            {
                Debug.LogWarning($"WorldPos {worldPosition} → grid [{gridX},{gridY}] is outside grid bounds.");
                return null;
            }

            return m_Grid[arrayX, arrayY];
        }

        public GridNode[] GetNeighbors(GridNode node)
        {
            List<GridNode> neighbors = new List<GridNode>();

            Vector2Int[] directions = m_AllowDiagonals ?
                new Vector2Int[]
                {
                    new Vector2Int(-1, 0), new Vector2Int(1, 0),
                    new Vector2Int(0, -1), new Vector2Int(0, 1),
                    new Vector2Int(-1, -1), new Vector2Int(1, -1),
                    new Vector2Int(-1, 1), new Vector2Int(1, 1)
                } :
                new Vector2Int[]
                {
                    new Vector2Int(-1, 0), new Vector2Int(1, 0),
                    new Vector2Int(0, -1), new Vector2Int(0, 1)
                };

            foreach (var dir in directions)
            {
                Vector2Int neighborPos = node.m_GridPos + dir;
                int x = neighborPos.x + m_GridOffset.x;
                int y = neighborPos.y + m_GridOffset.y;

                if (x < 0 || x >= m_GridSize.x || y < 0 || y >= m_GridSize.y)
                    continue;

                GridNode neighbor = m_Grid[x, y];
                if (neighbor.m_Walkable)
                    neighbors.Add(neighbor);
            }

            return neighbors.ToArray();
        }

        public bool IsSameNode(Vector3 a, Vector3 b)
        {
            return Mathf.RoundToInt(a.x) == Mathf.RoundToInt(b.x) &&
                   Mathf.RoundToInt(a.z) == Mathf.RoundToInt(b.z);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!m_EnableDebugDraw || m_Grid == null) return;

            foreach (var node in m_Grid)
            {
                Gizmos.color = Color.white;

                Vector3 pos = node.m_WorldPos + Vector3.up * 0.01f;
                Vector3 halfSize = Vector3.one * (m_CellSize * 0.5f);

                Vector3 p1 = pos + new Vector3(-halfSize.x, 0f, -halfSize.z);
                Vector3 p2 = pos + new Vector3(-halfSize.x, 0f, halfSize.z);
                Vector3 p3 = pos + new Vector3(halfSize.x, 0f, halfSize.z);
                Vector3 p4 = pos + new Vector3(halfSize.x, 0f, -halfSize.z);

                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p4);
                Gizmos.DrawLine(p4, p1);
            }

            if (m_ShowCoordinates)
            {
                foreach (GridNode node in m_Grid)
                {
                    Vector3 labelPos = node.m_WorldPos;

                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = node.m_Walkable ? (node.m_IsPathBlocked ? m_PathBlockedColor : m_WalkableColor) : m_UnwalkableColor;
                    style.fontSize = 10;

                    UnityEditor.Handles.Label(labelPos, $"[{node.m_GridPos.x},{node.m_GridPos.y}]", style);
                }
            }
        }
#endif
    }
}
