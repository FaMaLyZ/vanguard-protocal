using UnityEngine;

public class PlayerDesert : PlayerUnit
{
    public override void OnProjectileImpact(EnemyUnit target)
    {
        Vector2Int my = GridPos(transform.position);
        Vector2Int tgt = GridPos(target.transform.position);

        Vector2Int dir = new Vector2Int(
            Mathf.Clamp(tgt.x - my.x, -1, 1),
            Mathf.Clamp(tgt.y - my.y, -1, 1)
        );

        Vector2Int pushedTo = tgt + dir;

        if (GridManager.Instance.IsTileFree(pushedTo))
        {
            // ดัน 1 ช่อง
            target.ForceMoveTo(pushedTo);
        }
        else
        {
            // ชนศัตรู = damage +1
            if (GridManager.Instance.occupiedTiles.TryGetValue(pushedTo, out Unit hit))
            {
                if (hit is EnemyUnit e)
                    e.TakeDamage(1);
            }
        }

        // Desert ทำ damage ใหญ่ 1 hit ปกติด้วย
        target.TakeDamage(this.attackDamage);
    }

}
