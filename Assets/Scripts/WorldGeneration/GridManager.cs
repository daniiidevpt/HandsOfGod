using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject m_HexPrefab;
    [SerializeField] private int m_GridRadius = 10;
    [SerializeField] private float m_HexSize = 1f;

    private float m_HexWidth;
    private float m_HexHeight;

    public int GridRadius => m_GridRadius;
    public float HexSize  => m_HexSize;

    private void Awake()
    {
        InitializeGridValues();
        GenerateGrid();
    }

    private void InitializeGridValues()
    {
        m_HexWidth = m_HexSize * Mathf.Sqrt(3);
        m_HexHeight = m_HexSize * 1.5f;
    }

    private void GenerateGrid()
    {
        for (int q = -m_GridRadius; q <= m_GridRadius; q++)
        {
            for (int r = -m_GridRadius; r <= m_GridRadius; r++)
            {
                int s = -q - r;

                // Check distance for circle shape
                if (HexDistance(q, r, s) > m_GridRadius) continue;

                Vector3 worldPosition = AxialToWorld(q, r);

                GameObject hexInst = Instantiate(m_HexPrefab, worldPosition, Quaternion.identity);
                hexInst.transform.SetParent(transform, false);
                hexInst.name = $"Hex ({q}, {r})";
            }
        }
    }

    public int HexDistance(int q, int r, int s)
    {
        return Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s));
    }

    public Vector3 AxialToWorld(int q, int r)
    {
        float xPos = m_HexWidth * (q + r * 0.5f);
        float yPos = m_HexHeight * r;
        return new Vector3(xPos, 0, yPos);
    }
}
