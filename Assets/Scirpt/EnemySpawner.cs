using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float enemyHeight = 2.5f;

    public void SpawnEnemy()
    {
        if (!enemyPrefab)
        {
            Debug.LogError("Enemy Prefab is EMPTY!");
            return;
        }

        // พยายามหา tile ว่าง
        const int MAX_TRIES = 50;
        for (int i = 0; i < MAX_TRIES; i++)
        {
            int x = Random.Range(0, GridManager.Instance.width);
            int z = Random.Range(0, GridManager.Instance.height);
            Vector2Int gridPos = new Vector2Int(x, z);

            if (!GridManager.Instance.IsTileFree(gridPos)) continue;

            Vector3 worldPos = GridManager.Instance.GridToWorld(gridPos);
            worldPos.y = enemyHeight;

            GameObject enemyGo = Instantiate(enemyPrefab, worldPos, Quaternion.identity);

            // Occupy tile ทันที (Unit.Start จะทำซ้ำแต่ไม่เป็นไร/หรือจะลบก็ได้)
            GridManager.Instance.OccupyTile(gridPos, enemyGo.GetComponent<Unit>());

            EnemyUnit newEnemy = enemyGo.GetComponent<EnemyUnit>();
            if (newEnemy != null) GameManager.Instance.RegisterEnemyUnit(newEnemy);
            else Debug.LogError("Spawned prefab is missing EnemyUnit component!");

            Debug.Log($"Enemy spawned at Grid {gridPos} → World {worldPos}");
            return;
        }

        Debug.LogWarning("No free tile to spawn enemy.");
    }
}
