using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int m_GridRadius = 10;
    [SerializeField] private float m_HexSize = 1f;
    [SerializeField] private int m_Seed = 0;

    [Header("Lake Settings")]
    [SerializeField] private int m_MinLakes = 2;
    [SerializeField] private int m_MaxLakes = 4;
    [SerializeField] private int m_MinLakeSize = 2;
    [SerializeField] private int m_MaxLakeSize = 4;

    [Header("Zone Settings")]
    [SerializeField] private float m_ForestThreshold = 0.4f;
    [SerializeField] private float m_MountainThreshold = 0.7f;
    [SerializeField] private float m_VillageThreshold = 0.2f;

    [Header("Tile Prefabs")]
    [SerializeField] private List<GameObject> m_GrassPrefabs;
    [SerializeField] private List<GameObject> m_WaterPrefabs;
    [SerializeField] private List<GameObject> m_ForestPrefabs;
    [SerializeField] private List<GameObject> m_MountainPrefabs;
    [SerializeField] private List<GameObject> m_VillagePrefabs;

    private Dictionary<Vector2Int, TileData> m_TileMap = new Dictionary<Vector2Int, TileData>();
    private TerrainManager m_TerrainManager;

    private void Awake()
    {
        if (m_Seed == 0) m_Seed = Random.Range(1, 100000);
        Random.InitState(m_Seed);

        m_TerrainManager = new TerrainManager(
            m_GridRadius, m_HexSize,
            m_GrassPrefabs, m_WaterPrefabs,
            m_MinLakes, m_MaxLakes,
            m_MinLakeSize, m_MaxLakeSize,
            m_ForestThreshold, m_MountainThreshold, m_VillageThreshold,
            m_ForestPrefabs, m_MountainPrefabs, m_VillagePrefabs
        );

        m_TileMap = m_TerrainManager.GenerateTerrain();
        RefreshTilePrefabs();
    }

    private void RefreshTilePrefabs()
    {
        foreach (var tile in m_TileMap)
        {
            GameObject prefabToSpawn = SelectPrefab(tile.Value.Type);
            Instantiate(prefabToSpawn, tile.Value.WorldPosition, Quaternion.identity);

            if (tile.Value.Decoration != null)
                Instantiate(tile.Value.Decoration, tile.Value.WorldPosition + Vector3.up * 0.5f, Quaternion.identity);
        }
    }

    private GameObject SelectPrefab(TileType type)
    {
        if (type == TileType.Water)
            return m_WaterPrefabs[Random.Range(0, m_WaterPrefabs.Count)];
        return m_GrassPrefabs[Random.Range(0, m_GrassPrefabs.Count)];
    }
}
