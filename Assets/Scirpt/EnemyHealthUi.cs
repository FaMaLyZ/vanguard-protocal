using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyHealthUI : MonoBehaviour
{
    [System.Serializable]
    public class EnemyUIData
    {
        public Image enemyIcon;
        public Image[] hpIcons;
        [HideInInspector] public EnemyUnit enemy;
    }

    public EnemyUIData[] enemiesUI = new EnemyUIData[3];

    private List<EnemyUnit> spawnedEnemies = new List<EnemyUnit>();

    void Start()
    {
        InvokeRepeating(nameof(FindEnemies), 0f, 0.5f);
    }

    void Update()
    {
        UpdateHealthUI();
    }

    void FindEnemies()
    {
        EnemyUnit[] enemies = FindObjectsOfType<EnemyUnit>();

        if (enemies.Length == spawnedEnemies.Count)
            return;

        spawnedEnemies.Clear();
        spawnedEnemies.AddRange(enemies);

        for (int i = 0; i < enemiesUI.Length; i++)
        {
            if (i < spawnedEnemies.Count)
                enemiesUI[i].enemy = spawnedEnemies[i];
            else
                enemiesUI[i].enemy = null;
        }
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < enemiesUI.Length; i++)
        {
            var ui = enemiesUI[i];
            var enemy = ui.enemy;

            if (enemy == null)
            {
                foreach (var hpp in ui.hpIcons)
                    hpp.enabled = false;

                continue;
            }

            int hp = enemy.currentHealth;
            int maxHp = enemy.maxHealth;

            for (int h = 0; h < ui.hpIcons.Length; h++)
            {
                ui.hpIcons[h].gameObject.SetActive(h < maxHp);
                ui.hpIcons[h].enabled = h < hp;
            }
        }
    }
}
