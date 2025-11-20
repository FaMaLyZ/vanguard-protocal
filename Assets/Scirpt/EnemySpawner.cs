using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float enemyHeight = 2.5f;

    public GameObject spawnMarkerPrefab;
    private List<GameObject> spawnMarkers = new List<GameObject>();

    // ตำแหน่ง spawn แบบ FIX (อยากแก้ตรงนี้)
    public List<Vector2Int> fixedSpawnPoints = new List<Vector2Int>();


    public void SpawnEnemy()
    {
        if (!enemyPrefab)
        {
            Debug.LogError("Enemy Prefab is EMPTY!");
            return;
        }

        foreach (var gridPos in fixedSpawnPoints)
        {
            if (!GridManager.Instance.IsTileFree(gridPos)) continue;

            Vector3 worldPos = GridManager.Instance.GridToWorld(gridPos);
            worldPos.y = enemyHeight;

            GameObject enemyGo = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

            GridManager.Instance.OccupyTile(gridPos, enemyGo.GetComponent<Unit>());
            GameManager.Instance.RegisterEnemyUnit(enemyGo.GetComponent<EnemyUnit>());

            Debug.Log("Enemy Spawned at " + gridPos);
        }
    }

    public void ShowSpawnMarkers()
    {
        ClearSpawnMarkers();

        foreach (var pos in fixedSpawnPoints)
        {
            Vector3 world = GridManager.Instance.GridToWorld(pos);

            // วางบนผิว tile (Cube สูง tileSize → ผิวบน = tileSize * 0.5)
            world.y = GridManager.Instance.tileSize * 0.51f;

            GameObject marker = Instantiate(spawnMarkerPrefab, world, Quaternion.Euler(90, 0, 0));

            // ขยายให้เท่าช่อง Grid
            float t = GridManager.Instance.tileSize;
            marker.transform.localScale = new Vector3(t, t, t);

            spawnMarkers.Add(marker);
        }
    }



    public void ClearSpawnMarkers()
    {
        foreach (var m in spawnMarkers)
            Destroy(m);

        spawnMarkers.Clear();
    }



}
