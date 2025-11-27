using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObstacleSpawnData
{
    public GameObject obstaclePrefab;
    public Vector2Int gridPos;
}

public class ObstacleSpawner : MonoBehaviour
{
    public float obstacleHeight = 1.5f;

    // ⭐ ต้องมีบรรทัดนี้
    public List<ObstacleSpawnData> obstacleSpawns = new List<ObstacleSpawnData>();

    void Start()
    {
        SpawnFixedObstacles();
    }

    void SpawnFixedObstacles()
    {
        var grid = GridManager.Instance;

        foreach (var spawn in obstacleSpawns)
        {
            if (spawn.obstaclePrefab == null)
            {
                Debug.LogWarning("Obstacle prefab is missing in spawn list.");
                continue;
            }

            Vector2Int pos = spawn.gridPos;

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

            Quaternion rot = Quaternion.Euler(-90, 0, 0);
            GameObject obj = Instantiate(spawn.obstaclePrefab, worldPos, rot);

            grid.OccupyWithObstacle(pos, obj);
        }

        Debug.Log($"Spawned {obstacleSpawns.Count} obstacles.");
    }
}
