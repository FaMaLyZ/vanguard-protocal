using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 10;
    public int height = 10;
    public float tileSize = 3f;

    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;

    // world-center ของแต่ละช่อง
    public Dictionary<Vector2Int, Vector3> gridPositions = new Dictionary<Vector2Int, Vector3>();
    // ช่องที่ถูกยึดครอง
    public Dictionary<Vector2Int, Unit> occupiedTiles = new Dictionary<Vector2Int, Unit>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                bool isWhite = (x + z) % 2 == 0;
                GameObject prefab = isWhite ? whiteTilePrefab : blackTilePrefab;

                Vector3 pos = new Vector3(x * tileSize, 0, z * tileSize);
                GameObject tile = Instantiate(prefab, pos, Quaternion.identity, transform);
                tile.transform.localScale = Vector3.one * tileSize;

                gridPositions.Add(new Vector2Int(x, z), pos);
            }
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int z = Mathf.RoundToInt(worldPos.z / tileSize);
        return new Vector2Int(x, z);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * tileSize, 0, gridPos.y * tileSize);
    }

    public bool IsTileFree(Vector2Int gridPos) => !occupiedTiles.ContainsKey(gridPos);

    public void OccupyTile(Vector2Int gridPos, Unit unit) => occupiedTiles[gridPos] = unit;

    public void FreeTile(Vector2Int gridPos)
    {
        if (occupiedTiles.ContainsKey(gridPos)) occupiedTiles.Remove(gridPos);
    }
}
