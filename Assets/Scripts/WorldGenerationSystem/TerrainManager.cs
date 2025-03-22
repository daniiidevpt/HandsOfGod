using UnityEngine;
using System.Collections.Generic;

public class TerrainManager
{
    private int m_GridRadius;
    private float m_HexSize;

    private List<GameObject> m_GrassPrefabs;
    private List<GameObject> m_WaterPrefabs;
    private List<GameObject> m_ForestPrefabs;
    private List<GameObject> m_MountainPrefabs;
    private List<GameObject> m_VillagePrefabs;

    private int m_MinLakes;
    private int m_MaxLakes;
    private int m_MinLakeSize;
    private int m_MaxLakeSize;

    private float m_ForestThreshold;
    private float m_MountainThreshold;
    private float m_VillageThreshold;

    private float m_HexWidth;
    private float m_HexHeight;

    private float m_NoiseOffsetX;
    private float m_NoiseOffsetY;

    public TerrainManager(
        int gridRadius, float hexSize,
        List<GameObject> grassPrefabs, List<GameObject> waterPrefabs,
        int minLakes, int maxLakes,
        int minLakeSize, int maxLakeSize,
        float forestThreshold, float mountainThreshold, float villageThreshold,
        List<GameObject> forestPrefabs, List<GameObject> mountainPrefabs, List<GameObject> villagePrefabs)
    {
        m_GridRadius = gridRadius;
        m_HexSize = hexSize;
        m_GrassPrefabs = grassPrefabs;
        m_WaterPrefabs = waterPrefabs;
        m_ForestPrefabs = forestPrefabs;
        m_MountainPrefabs = mountainPrefabs;
        m_VillagePrefabs = villagePrefabs;

        m_MinLakes = minLakes;
        m_MaxLakes = maxLakes;
        m_MinLakeSize = minLakeSize;
        m_MaxLakeSize = maxLakeSize;

        m_ForestThreshold = forestThreshold;
        m_MountainThreshold = mountainThreshold;
        m_VillageThreshold = villageThreshold;

        m_HexWidth = m_HexSize * Mathf.Sqrt(3);
        m_HexHeight = m_HexSize * 1.5f;

        m_NoiseOffsetX = Random.Range(0f, 1000f);
        m_NoiseOffsetY = Random.Range(0f, 1000f);
    }

    public Dictionary<Vector2Int, TileData> GenerateTerrain()
    {
        Dictionary<Vector2Int, TileData> tileMap = new Dictionary<Vector2Int, TileData>();

        for (int q = -m_GridRadius; q <= m_GridRadius; q++)
        {
            for (int r = -m_GridRadius; r <= m_GridRadius; r++)
            {
                int s = -q - r;
                if (HexDistance(q, r, s) > m_GridRadius) continue;

                Vector2Int hexCoord = new Vector2Int(q, r);
                Vector3 worldPosition = AxialToWorld(q, r);

                tileMap[hexCoord] = new TileData(hexCoord, worldPosition, TileType.Grassland);
            }
        }

        GenerateLakes(tileMap);
        GenerateZones(tileMap);
        return tileMap;
    }

    private void GenerateLakes(Dictionary<Vector2Int, TileData> tileMap)
    {
        int numberOfLakes = Random.Range(m_MinLakes, m_MaxLakes + 1);

        for (int i = 0; i < numberOfLakes; i++)
        {
            Vector2Int lakeCenter = GetValidLakePosition(tileMap);
            CreateLake(lakeCenter, tileMap);
        }
    }

    private Vector2Int GetValidLakePosition(Dictionary<Vector2Int, TileData> tileMap)
    {
        int safeRadius = m_GridRadius - 3;
        List<Vector2Int> validTiles = new List<Vector2Int>();

        foreach (var tile in tileMap)
        {
            if (tile.Value.Type == TileType.Grassland && HexDistance(tile.Key.x, tile.Key.y, -tile.Key.x - tile.Key.y) <= safeRadius)
                validTiles.Add(tile.Key);
        }

        return validTiles.Count > 0 ? validTiles[Random.Range(0, validTiles.Count)] : Vector2Int.zero;
    }

    private void CreateLake(Vector2Int center, Dictionary<Vector2Int, TileData> tileMap)
    {
        int lakeSize = Random.Range(m_MinLakeSize, m_MaxLakeSize + 1);
        HashSet<Vector2Int> lakeTiles = new HashSet<Vector2Int> { center };

        while (lakeTiles.Count < lakeSize)
        {
            Vector2Int randomTile = GetRandomTileFromSet(lakeTiles);
            Vector2Int randomNeighbor = GetRandomNeighbor(randomTile, tileMap);

            if (randomNeighbor != Vector2Int.zero)
                lakeTiles.Add(randomNeighbor);
        }

        foreach (var tile in lakeTiles)
        {
            if (tileMap.ContainsKey(tile))
                tileMap[tile].Type = TileType.Water;
        }
    }

    private void GenerateZones(Dictionary<Vector2Int, TileData> tileMap)
    {
        Dictionary<Vector2Int, float> noiseMap = new Dictionary<Vector2Int, float>();

        foreach (var tile in tileMap)
        {
            float noiseValue = Mathf.PerlinNoise(
                (tile.Key.x * 0.1f) + m_NoiseOffsetX,
                (tile.Key.y * 0.1f) + m_NoiseOffsetY
            );
            noiseMap[tile.Key] = noiseValue;
        }

        int numMountainZones = Random.Range(1, 3);  // 1 to 2 mountain zones
        int numForestZones = Random.Range(2, 4);  // 2 to 3 forest zones
        int numVillageZones = Random.Range(1, 2);  // 1 to 2 village zones

        for (int i = 0; i < numMountainZones; i++)
        {
            Vector2Int mountainCenter = GetRandomHighNoiseTile(noiseMap, tileMap);
            ExpandZone(tileMap, mountainCenter, Random.Range(4, 8), m_MountainPrefabs);
        }

        for (int i = 0; i < numForestZones; i++)
        {
            Vector2Int forestCenter = GetRandomMidNoiseTile(noiseMap, tileMap);
            ExpandZone(tileMap, forestCenter, Random.Range(6, 12), m_ForestPrefabs);
        }

        for (int i = 0; i < numVillageZones; i++)
        {
            Vector2Int villageCenter = GetRandomLowNoiseTile(noiseMap, tileMap);
            ExpandZone(tileMap, villageCenter, Random.Range(3, 6), m_VillagePrefabs);
        }
    }


    private Vector2Int GetRandomHighNoiseTile(Dictionary<Vector2Int, float> noiseMap, Dictionary<Vector2Int, TileData> tileMap)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        float threshold = 0.75f;  // Define what is "high" noise

        foreach (var tile in noiseMap)
        {
            if (tile.Value > threshold && tileMap[tile.Key].Type != TileType.Water)
                candidates.Add(tile.Key);
        }

        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : Vector2Int.zero;
    }

    private Vector2Int GetRandomMidNoiseTile(Dictionary<Vector2Int, float> noiseMap, Dictionary<Vector2Int, TileData> tileMap)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        float minThreshold = 0.4f;
        float maxThreshold = 0.7f;

        foreach (var tile in noiseMap)
        {
            if (tile.Value > minThreshold && tile.Value < maxThreshold && tileMap[tile.Key].Type != TileType.Water)
                candidates.Add(tile.Key);
        }

        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : Vector2Int.zero;
    }

    private Vector2Int GetRandomLowNoiseTile(Dictionary<Vector2Int, float> noiseMap, Dictionary<Vector2Int, TileData> tileMap)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        float threshold = 0.3f;  // Define what is "low" noise

        foreach (var tile in noiseMap)
        {
            if (tile.Value < threshold && tileMap[tile.Key].Type != TileType.Water)
                candidates.Add(tile.Key);
        }

        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : Vector2Int.zero;
    }


    private void ExpandZone(Dictionary<Vector2Int, TileData> tileMap, Vector2Int center, int size, List<GameObject> prefabs)
    {
        HashSet<Vector2Int> zoneTiles = new HashSet<Vector2Int> { center };

        while (zoneTiles.Count < size)
        {
            Vector2Int randomTile = GetRandomTileFromSet(zoneTiles);
            Vector2Int randomNeighbor = GetRandomNeighbor(randomTile, tileMap);

            if (randomNeighbor != Vector2Int.zero && !zoneTiles.Contains(randomNeighbor))
            {
                if (Random.value > 0.2f)  // 80% chance to expand in a certain direction
                    zoneTiles.Add(randomNeighbor);
            }
        }

        foreach (var tile in zoneTiles)
        {
            if (tileMap.ContainsKey(tile) && tileMap[tile].Type == TileType.Grassland)
                tileMap[tile].Decoration = prefabs[Random.Range(0, prefabs.Count)];
        }
    }



    private Vector2Int GetRandomTileFromSet(HashSet<Vector2Int> tileSet)
    {
        int index = Random.Range(0, tileSet.Count);
        foreach (Vector2Int tile in tileSet)
        {
            if (index == 0) return tile;
            index--;
        }
        return Vector2Int.zero;
    }

    private Vector2Int GetRandomNeighbor(Vector2Int tile, Dictionary<Vector2Int, TileData> tileMap)
    {
        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, -1), new Vector2Int(-1, 1)
        };

        List<Vector2Int> validNeighbors = new List<Vector2Int>();

        foreach (var dir in directions)
        {
            Vector2Int neighbor = tile + dir;
            if (tileMap.ContainsKey(neighbor) && tileMap[neighbor].Type == TileType.Grassland)
                validNeighbors.Add(neighbor);
        }

        return validNeighbors.Count > 0 ? validNeighbors[Random.Range(0, validNeighbors.Count)] : Vector2Int.zero;
    }

    private int HexDistance(int q, int r, int s)
    {
        return Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s));
    }

    private Vector3 AxialToWorld(int q, int r)
    {
        float xPos = m_HexWidth * (q + r * 0.5f);
        float yPos = m_HexHeight * r;
        return new Vector3(xPos, 0, yPos);
    }
}
