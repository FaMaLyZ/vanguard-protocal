using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public int obstacleCount = 10; // จำนวน Obstacle ที่อยากวางตอนเริ่มเกม
    public float obstacleHeight = 1.5f;

    void Start()
    {
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        var grid = GridManager.Instance;

        int tries = 0;
        int spawned = 0;
        int MAX_TRIES = obstacleCount * 10; // กันลูปค้าง

        while (spawned < obstacleCount && tries < MAX_TRIES)
        {
            tries++;

            int x = Random.Range(0, grid.width);
            int y = Random.Range(0, grid.height);
            Vector2Int gridPos = new Vector2Int(x, y);

            // ห้ามวางบนช่องที่มี unit หรือ obstacle อยู่แล้ว
            if (!grid.IsTileFree(gridPos))
                continue;

            // ตำแหน่ง world (snap กลาง tile)
            Vector3 worldPos = grid.GridToWorld(gridPos);
            worldPos.y = obstacleHeight;

            GameObject obj = Instantiate(obstaclePrefab, worldPos, Quaternion.identity);

            // บันทึก obstacle ลง grid
            grid.OccupyWithObstacle(gridPos, obj);

            spawned++;
        }

        Debug.Log($"Spawned {spawned} obstacles.");
    }
}
