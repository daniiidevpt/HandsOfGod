using UnityEngine;

public enum TileType { Grassland, Water }

public class TileData
{
    public Vector2Int AxialCoords;
    public Vector3 WorldPosition;
    public TileType Type;
    public GameObject Decoration;

    public TileData(Vector2Int coords, Vector3 worldPos, TileType type)
    {
        AxialCoords = coords;
        WorldPosition = worldPos;
        Type = type;
        Decoration = null;
    }
}
