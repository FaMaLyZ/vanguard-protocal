using UnityEngine;

public class PlayerCrimson : PlayerUnit
{
    public int aoeRadius = 1;

    public override void OnProjectileImpact(EnemyUnit target)
    {
        Vector2Int center = GridPos(target.transform.position);

        // damage AoE 1 tile รอบตัว (เหมือนเดิม)
        for (int dx = -aoeRadius; dx <= aoeRadius; dx++)
        {
            for (int dz = -aoeRadius; dz <= aoeRadius; dz++)
            {
                Vector2Int pos = new Vector2Int(center.x + dx, center.y + dz);

                if (GridManager.Instance.occupiedTiles.TryGetValue(pos, out Unit u))
                {
                    if (u is EnemyUnit e)
                        e.TakeDamage(this.attackDamage);
                }
            }
        }
    }
}
