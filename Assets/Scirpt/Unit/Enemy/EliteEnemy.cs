using UnityEngine;

public class EliteEnemy : EnemyUnit
{
    private Vector2Int[] AoEOffsets =
    {
        new Vector2Int( 0, 0),
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int( 0, 1),
        new Vector2Int( 0,-1),
        new Vector2Int( 1, 1),
        new Vector2Int( 1,-1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1,-1),
    };

    // -----------------------------------------
    // 1) Preview Phase (หลัง MovePhase)
    // -----------------------------------------
    public override void ShowAttackPreview()
    {
        var grid = GridManager.Instance;
        Vector2Int center = grid.WorldToGrid(transform.position);

        foreach (var o in AoEOffsets)
        {
            Vector2Int cell = center + o;
            if (grid.InBounds(cell))
                grid.HighlightEnemyPreview(cell);
        }
    }

    // -----------------------------------------
    // 2) Attack Phase (ต้นรอบ Enemy)
    // -----------------------------------------
    public override void ExecutePlannedAttack()
    {
        var grid = GridManager.Instance;
        Vector2Int center = grid.WorldToGrid(transform.position);

        foreach (var o in AoEOffsets)
        {
            Vector2Int cell = center + o;

            if (grid.occupiedTiles.TryGetValue(cell, out Unit u))
            {
                if (u is PlayerUnit p)
                {
                    p.TakeDamage(attackDamage);
                    Debug.Log($"{name} AoE hit {p.name} at {cell}");
                }
            }
        }
    }
}
