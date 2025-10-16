using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    

    public void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab variable is EMPTY in the Inspector!");
            return;
        }

        int x = Random.Range(0, 21);
        int z = Random.Range(0, 21);
        Vector3 pos = new Vector3(x, 2.5f, z);
        
        GameObject enemyGo = Instantiate(enemyPrefab, pos,Quaternion.identity);

        EnemyUnit newEnemy = enemyGo.GetComponent<EnemyUnit>();

        if(newEnemy != null )
        {
            GameManager.Instance.RegisterEnemyUnit(newEnemy);
        }
        else
        {
            Debug.LogError($"Spawned prefab is missing an EnemyUnit component!");
        }
    }

}