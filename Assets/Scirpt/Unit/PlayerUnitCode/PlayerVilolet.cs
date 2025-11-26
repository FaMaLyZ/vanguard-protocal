using UnityEngine;

public class VioletUnit : PlayerUnit
{
    public override void Attack(EnemyUnit target)
    {
        Vector2Int center = GridPos(target.transform.position);

        for (int dx = -1; dx <= 1; dx++)
            for (int dz = -1; dz <= 1; dz++)
            {
                Vector2Int pos = new Vector2Int(center.x + dx, center.y + dz);

                if (GridManager.Instance.occupiedTiles.TryGetValue(pos, out Unit u))
                {
                    if (u is EnemyUnit e)
                    {
                        Vector2Int pullDir = center - pos;     // ดึงเข้าหาศูนย์กลาง
                        PullEnemy(e, pullDir);
                    }
                }
            }

        base.Attack(target);
    }

    void PullEnemy(EnemyUnit enemy, Vector2Int dir)
    {
        Vector2Int enemyCurrentPos = GridPos(enemy.transform.position);
        Vector2Int newPos = enemyCurrentPos + dir;


        // ขยับศัตรูแบบปลอดภัย
        if (GridManager.Instance.IsTileFree(newPos))
        {
            enemy.ForceMoveTo(newPos);     // <<< ใช้อันนี้แทนทั้งหมด
        }
        else
        {
            // ชนตัวอื่น = ทำอะไรเพิ่มได้ เช่น damage
            if (GridManager.Instance.occupiedTiles.TryGetValue(newPos, out Unit hit))
            {
                if (hit is EnemyUnit e)
                    e.TakeDamage(1);
            }
        }
    }
}
