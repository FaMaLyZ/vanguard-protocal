using UnityEngine;

public class PlayerVilolet : PlayerUnit
{
    public override void OnProjectileImpact(EnemyUnit target)
    {
        Vector2Int center = GridPos(target.transform.position);

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                Vector2Int pos = new Vector2Int(center.x + dx, center.y + dz);

                if (GridManager.Instance.occupiedTiles.TryGetValue(pos, out Unit u))
                {
                    if (u is EnemyUnit e)
                    {
                        Vector2Int pullDir = center - pos;
                        PullEnemy(e, pullDir);
                    }
                }
            }
        }

        // Violet ยังทำ damage ปกติให้ target
        target.TakeDamage(this.attackDamage);
    }

    void PullEnemy(EnemyUnit enemy, Vector2Int dir)
    {
        Vector2Int cur = GridPos(enemy.transform.position);
        Vector2Int newPos = cur + dir;

        if (GridManager.Instance.IsTileFree(newPos))
        {
            enemy.ForceMoveTo(newPos);
        }
        else
        {
            if (GridManager.Instance.occupiedTiles.TryGetValue(newPos, out Unit hit))
            {
                if (hit is EnemyUnit e)
                    e.TakeDamage(1);
            }
        }
    }
}
