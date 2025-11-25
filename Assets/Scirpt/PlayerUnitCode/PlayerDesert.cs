using UnityEngine;

public class DesertUnit : PlayerUnit
{
    public override void Attack(EnemyUnit target)
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
            // move enemy 1 tile
            target.ForceMoveTo(pushedTo);
        }
        else
        {
            // collision damage
            if (GridManager.Instance.occupiedTiles.TryGetValue(pushedTo, out Unit u))
            {
                if (u is EnemyUnit e)
                    e.TakeDamage(1);
            }
        }

        base.Attack(target);
    }

}
