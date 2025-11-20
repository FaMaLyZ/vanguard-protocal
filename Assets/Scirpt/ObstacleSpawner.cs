using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float obstacleHeight = 1.5f;

    // 💡 Dev สามารถกำหนดตำแหน่ง Obstacle เองใน Inspector
    public List<Vector2Int> fixedObstaclePositions = new List<Vector2Int>();

    void Start()
    {
        SpawnFixedObstacles();
    }

    void SpawnFixedObstacles()
    {
        var grid = GridManager.Instance;

        foreach (var pos in fixedObstaclePositions)
        {
            if (!grid.InBounds(pos))
            {
                Debug.LogWarning($"Obstacle position {pos} is out of grid!");
                continue;
            }

            if (!grid.IsTileFree(pos))
            {
                Debug.LogWarning($"Tile {pos} is not free. Skipping obstacle.");
                continue;
            }

            Vector3 worldPos = grid.GridToWorld(pos);
            worldPos.y = obstacleHeight;

            GameObject obj = Instantiate(obstaclePrefab, worldPos, Quaternion.identity);

            grid.OccupyWithObstacle(pos, obj);
        }

        Debug.Log($"Spawned {fixedObstaclePositions.Count} fixed obstacles.");
    }
}
