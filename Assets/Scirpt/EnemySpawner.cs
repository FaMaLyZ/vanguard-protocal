using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct EnemySpawnPoint
    {
        public GameObject enemyPrefab;   // จะ spawn อะไร
        public Vector2Int gridPos;       // จะ spawn ช่องไหน
    }
    [Header("Marker Settings")]
    public float markerScale = 1f;
    public float enemyHeight = 2.5f;

    public GameObject spawnMarkerPrefab;
    private List<GameObject> spawnMarkers = new List<GameObject>();

    // ตำแหน่ง spawn แบบ FIX (อยากแก้ตรงนี้)
    public List<EnemySpawnPoint> spawnPoints = new List<EnemySpawnPoint>();


    public void SpawnEnemy()
    {
        foreach (var sp in spawnPoints)
        {
            if (sp.enemyPrefab == null)
            {
                Debug.LogWarning("Enemy prefab is null!");
                continue;
            }

            if (!GridManager.Instance.IsTileFree(sp.gridPos))
            {
                Debug.LogWarning($"Tile {sp.gridPos} is occupied!");
                continue;
            }

            Vector3 worldPos = GridManager.Instance.GridToWorld(sp.gridPos);
            worldPos.y = enemyHeight;

            GameObject enemyGo = Instantiate(sp.enemyPrefab, worldPos, Quaternion.identity);

            GridManager.Instance.OccupyTile(sp.gridPos, enemyGo.GetComponent<Unit>());
            GameManager.Instance.RegisterEnemyUnit(enemyGo.GetComponent<EnemyUnit>());

            Debug.Log($"Spawned {sp.enemyPrefab.name} at {sp.gridPos}");
        }
    }


    public void ShowSpawnMarkers()
    {
        ClearSpawnMarkers();

        foreach (var sp in spawnPoints)
        {
            Vector3 world = GridManager.Instance.GridToWorld(sp.gridPos);
            world.y = GridManager.Instance.tileSize * 0.8f;

            GameObject marker = Instantiate(spawnMarkerPrefab, world, Quaternion.identity);

            float t = GridManager.Instance.tileSize;
            marker.transform.localScale = Vector3.one * markerScale;


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
