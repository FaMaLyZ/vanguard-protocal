using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [System.Serializable]
    public class ShipHPUI
    {
        public Image shipIcon;
        public Image[] hpIcons;
        public PlayerUnit player;
    }

    public ShipHPUI[] ships;  // 3 ลำ

    void Update()
    {
        foreach (var s in ships)
        {
            if (s.player == null) continue;

            int hp = s.player.currentHealth;      // HP จริงของ player
            int maxHp = s.player.maxHealth;       // HP สูงสุด

            for (int i = 0; i < s.hpIcons.Length; i++)
            {
                s.hpIcons[i].enabled = i < hp; // เปิด icon เท่ากับ HP ที่เหลือ
            }
        }
    }
}
