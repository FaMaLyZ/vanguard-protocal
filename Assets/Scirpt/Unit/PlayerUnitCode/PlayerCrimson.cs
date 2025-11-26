using UnityEngine;

public class CrimsonUnit : PlayerUnit
{
    public int explosionRadius = 1; // 3x3

    public override void Attack(EnemyUnit target)
    {
        Vector2Int impact = GridPos(target.transform.position);

        for (int dx = -explosionRadius; dx <= explosionRadius; dx++)
            for (int dz = -explosionRadius; dz <= explosionRadius; dz++)
            {
                Vector2Int pos = new Vector2Int(impact.x + dx, impact.y + dz);

                if (GridManager.Instance.occupiedTiles.TryGetValue(pos, out Unit u))
                    if (u is EnemyUnit e)
                        e.TakeDamage(attackDamage);
            }

        // ยิงปกติเป็น visual
        base.Attack(target);
    }
}
