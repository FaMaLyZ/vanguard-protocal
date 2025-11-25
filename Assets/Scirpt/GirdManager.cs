using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 10;
    public int height = 10;
    public float tileSize = 3f;   // ✅ ปรับขนาด tile ได้จาก Inspector

    public GameObject whiteTilePrefab;
    public GameObject blackTilePrefab;

    public GameObject greenHighlightPrefab;
    public GameObject redHighlightPrefab;


    private List<GameObject> activeHighlights = new List<GameObject>();

    private List<GameObject> enemyPreviewTiles = new List<GameObject>();
    public GameObject enemyPreviewPrefab;// ให้เลือก prefab สีแดงสำหรับ enemy



    // เก็บตำแหน่ง world ของแต่ละ tile
    public Dictionary<Vector2Int, Vector3> gridPositions = new Dictionary<Vector2Int, Vector3>();


    public Dictionary<Vector2Int, Unit> occupiedTiles = new Dictionary<Vector2Int, Unit>();

    public Dictionary<Vector2Int, GameObject> obstacleTiles = new Dictionary<Vector2Int, GameObject>();

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

                Vector3 pos = new Vector3(
                    x * tileSize,
                    0,
                    z * tileSize
                );

                GameObject tile = Instantiate(prefab, pos, Quaternion.identity, transform);
                tile.name = $"Tile ({x},{z})";
                tile.transform.localScale = Vector3.one * tileSize;

                gridPositions.Add(new Vector2Int(x, z), pos);
            }
        }
    }

    // ✅ world → grid
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int z = Mathf.RoundToInt(worldPos.z / tileSize);
        return new Vector2Int(x, z);
    }

    // ✅ grid → world
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        float y = tileSize * 0.5f;   // = 1.5 ถ้า tileSize = 3

        return new Vector3(
            gridPos.x * tileSize,
            y,
            gridPos.y * tileSize
        );
    }



    // ✅ ช่องนี้ว่างไหม?
    public bool IsTileFree(Vector2Int gridPos)
    {
        if (occupiedTiles.ContainsKey(gridPos)) return false;
        if (obstacleTiles.ContainsKey(gridPos)) return false;
        return true;
    }

    // ✅ จองช่อง
    public void OccupyTile(Vector2Int gridPos, Unit unit)
    {
        occupiedTiles[gridPos] = unit;
    }

    // ✅ เคลียร์ช่อง
    public void FreeTile(Vector2Int gridPos)
    {
        if (occupiedTiles.ContainsKey(gridPos)) occupiedTiles.Remove(gridPos);
    }
    // อยู่ในคลาส GridManager
    public bool InBounds(Vector2Int g)
    {
        return g.x >= 0 && g.x < width && g.y >= 0 && g.y < height;
    }
    // เพื่อนบ้าน 4 ทิศ
    public List<Vector2Int> GetNeighbors4(Vector2Int g)
    {
        var n = new List<Vector2Int>(4)
    {
        new Vector2Int(g.x + 1, g.y),
        new Vector2Int(g.x - 1, g.y),
        new Vector2Int(g.x, g.y + 1),
        new Vector2Int(g.x, g.y - 1),
    };
        // เฉพาะที่ยังอยู่ในกระดาน
        n.RemoveAll(p => !InBounds(p));
        return n;
    }
    // หา "ช่องว่าง" ที่ใกล้ที่สุดรอบๆ center (ขยายรัศมีทีละชั้น)
    public bool TryFindNearestFreeTile(Vector2Int center, Vector2Int preferFrom, int maxRadius, out Vector2Int result)
    {
        // ชั้นแรก: รอบเป้าหมายก่อน (ระยะ 1)
        var candidates = new List<Vector2Int>();
        for (int r = 1; r <= maxRadius; r++)
        {
            candidates.Clear();

            for (int dx = -r; dx <= r; dx++)
            {
                int dy = r - Mathf.Abs(dx);

                // จุดบน "ขอบรูปสี่เหลี่ยมข้าวหลามตัด" ระยะ r
                var p1 = new Vector2Int(center.x + dx, center.y + dy);
                var p2 = new Vector2Int(center.x + dx, center.y - dy);

                if (dy == 0)
                {
                    if (InBounds(p1) && IsTileFree(p1)) candidates.Add(p1);
                }
                else
                {
                    if (InBounds(p1) && IsTileFree(p1)) candidates.Add(p1);
                    if (InBounds(p2) && IsTileFree(p2)) candidates.Add(p2);
                }
            }

            if (candidates.Count > 0)
            {
                // เลือกตัวที่ "ใกล้ศัตรู" มากที่สุด เพื่อให้เดินสั้น/ไม่อ้อมเกิน
                candidates.Sort((a, b) =>
                {
                    int da = Mathf.Abs(a.x - preferFrom.x) + Mathf.Abs(a.y - preferFrom.y);
                    int db = Mathf.Abs(b.x - preferFrom.x) + Mathf.Abs(b.y - preferFrom.y);
                    return da.CompareTo(db);
                });

                result = candidates[0];
                return true;
            }
        }

        result = default;
        return false;
    }
    public bool IsObstacleAt(Vector2Int gridPos)
    {
        return obstacleTiles.ContainsKey(gridPos);
    }
    public void OccupyWithObstacle(Vector2Int gridPos, GameObject obstacle)
    {
        obstacleTiles[gridPos] = obstacle;
    }
    public void FreeObstacle(Vector2Int gridPos)
    {
        if (obstacleTiles.ContainsKey(gridPos)) obstacleTiles.Remove(gridPos);
    }
    public void HighlightTile(Vector2Int gridPos, Color color)
    {
        GameObject prefab = color == Color.red ? redHighlightPrefab : greenHighlightPrefab;

        Vector3 world = GridToWorld(gridPos);

        // 👈 ยกขึ้นบนผิว tile จริง ๆ
        world.y = (tileSize * 0.5f) + 0.02f;

        GameObject h = Instantiate(prefab, world, Quaternion.identity);
        activeHighlights.Add(h);

        var hl = h.GetComponent<TileHighlight>();
        if (hl != null) hl.gridPos = gridPos;
    }
    public void ClearHighlights()
    {
        foreach (var h in activeHighlights)
            if (h != null) Destroy(h);

        activeHighlights.Clear();
    }
    public void HighlightEnemyPreview(Vector2Int gridPos)
    {
        Vector3 world = GridToWorld(gridPos);
        world.y = (tileSize * 0.5f) + 0.02f;

        GameObject h = Instantiate(enemyPreviewPrefab, world, Quaternion.Euler(90, 0, 0));
        h.transform.localScale = Vector3.one * tileSize;

        enemyPreviewTiles.Add(h);
    }
    public void ClearEnemyPreview()
    {
        foreach (var h in enemyPreviewTiles)
            if (h != null) Destroy(h);

        enemyPreviewTiles.Clear();
    }


}
