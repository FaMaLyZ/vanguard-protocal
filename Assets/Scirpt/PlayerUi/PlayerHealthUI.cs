using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    [System.Serializable]
    public class ShipHealthUI
    {
        public Image shipIcon;
        public Image[] hpIcons;
        [HideInInspector] public PlayerUnit player;  // จะเซ็ตตอน runtime
    }

    public ShipHealthUI[] shipsUI = new ShipHealthUI[3];  // UI ของ 3 ลำ
    private List<PlayerUnit> spawnedPlayers = new List<PlayerUnit>();

    void Start()
    {
        InvokeRepeating(nameof(FindPlayers), 0f, 0.5f);  // คอยหา player ทุก 0.5 วิ จนกว่าจะครบ
    }

    void Update()
    {
        UpdateHealthUI();
    }

    void FindPlayers()
    {
        // หา PlayerUnit ที่ spawn ใหม่ในฉาก
        PlayerUnit[] players = FindObjectsOfType<PlayerUnit>();

        if (players.Length == spawnedPlayers.Count) 
            return; // ถ้าเจอเท่าเดิม ไม่ต้องทำอะไร

        spawnedPlayers.Clear();
        spawnedPlayers.AddRange(players);

        // จำกัดให้มีแค่ 3 ลำ ตาม UI ที่เตรียมไว้
        for (int i = 0; i < shipsUI.Length; i++)
        {
            if (i < spawnedPlayers.Count)
                shipsUI[i].player = spawnedPlayers[i];       // เซ็ต player ให้ UI
            else
                shipsUI[i].player = null;                    // เคลียร์ถ้า player ไม่มี
        }
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < shipsUI.Length; i++)
        {
            var ui = shipsUI[i];
            var player = ui.player;

            if (player == null)
            {
                // ปิด HP UI ถ้า player ยังไม่ spawn หรือถูกฆ่าแล้ว
                foreach (var hpp in ui.hpIcons)
                    hpp.enabled = false;

                continue;
            }

            int hp = player.currentHealth;
            int maxHp = player.maxHealth;

            for (int h = 0; h < ui.hpIcons.Length; h++)
            {
                ui.hpIcons[h].gameObject.SetActive(h < maxHp);
                ui.hpIcons[h].enabled = h < hp;
            }
        }
    }
}
