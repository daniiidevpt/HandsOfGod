//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
//public class GridCollider : MonoBehaviour
//{
//    [Header("Collider Settings")]
//    [SerializeField] private float m_ColliderHeight = 0.1f;

//    private MeshFilter m_MeshFilter;
//    private MeshCollider m_MeshCollider;
//    private GridManager m_Grid;

//    private void Start()
//    {
//        m_Grid = GetComponent<GridManager>();
//        m_MeshFilter = GetComponent<MeshFilter>();
//        m_MeshCollider = GetComponent<MeshCollider>();

//        GenerateMesh();
//    }

//    private void GenerateMesh()
//    {
//        List<Vector3> vertices = new List<Vector3>();
//        List<int> triangles = new List<int>();

//        Dictionary<Vector3, int> vertexMap = new Dictionary<Vector3, int>();

//        foreach (var hex in GetHexPositions())
//        {
//            Vector3 center = new Vector3(hex.x, 0, hex.z);
//            Vector3 centerBottom = center - new Vector3(0, m_ColliderHeight, 0); // Bottom face

//            for (int i = 0; i < 6; i++)
//            {
//                Vector3 cornerA = center + GetHexCorner(i);
//                Vector3 cornerB = center + GetHexCorner((i + 1) % 6);
//                Vector3 cornerA_Bottom = cornerA - new Vector3(0, m_ColliderHeight, 0);
//                Vector3 cornerB_Bottom = cornerB - new Vector3(0, m_ColliderHeight, 0);

//                // Add top hex face
//                int a = GetOrAddVertex(vertexMap, vertices, cornerA);
//                int b = GetOrAddVertex(vertexMap, vertices, cornerB);
//                int c = GetOrAddVertex(vertexMap, vertices, center);
//                triangles.Add(a);
//                triangles.Add(b);
//                triangles.Add(c);

//                // Add bottom hex face (inverted)
//                int aB = GetOrAddVertex(vertexMap, vertices, cornerA_Bottom);
//                int bB = GetOrAddVertex(vertexMap, vertices, cornerB_Bottom);
//                int cB = GetOrAddVertex(vertexMap, vertices, centerBottom);
//                triangles.Add(cB);
//                triangles.Add(bB);
//                triangles.Add(aB);

//                // Add side faces
//                triangles.Add(a);
//                triangles.Add(aB);
//                triangles.Add(b);

//                triangles.Add(b);
//                triangles.Add(aB);
//                triangles.Add(bB);
//            }
//        }

//        Mesh mesh = new Mesh();
//        mesh.vertices = vertices.ToArray();
//        mesh.triangles = triangles.ToArray();
//        mesh.RecalculateNormals();

//        m_MeshFilter.mesh = mesh;
//        m_MeshCollider.sharedMesh = mesh;
//    }

//    private int GetOrAddVertex(Dictionary<Vector3, int> vertexMap, List<Vector3> vertices, Vector3 vertex)
//    {
//        if (!vertexMap.ContainsKey(vertex))
//        {
//            vertexMap[vertex] = vertices.Count;
//            vertices.Add(vertex);
//        }
//        return vertexMap[vertex];
//    }

//    private List<Vector3> GetHexPositions()
//    {
//        List<Vector3> hexPositions = new List<Vector3>();

//        for (int q = -m_Grid.GridRadius; q <= m_Grid.GridRadius; q++)
//        {
//            for (int r = -m_Grid.GridRadius; r <= m_Grid.GridRadius; r++)
//            {
//                int s = -q - r;
//                if (Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s)) > m_Grid.GridRadius) continue;

//                hexPositions.Add(m_Grid.AxialToWorld(q, r));
//            }
//        }

//        return hexPositions;
//    }

//    private Vector3 GetHexCorner(int corner)
//    {
//        float angle = 60 * corner * Mathf.Deg2Rad;
//        return new Vector3(m_Grid.HexSize * Mathf.Cos(angle), 0, m_Grid.HexSize * Mathf.Sin(angle));
//    }
//}
