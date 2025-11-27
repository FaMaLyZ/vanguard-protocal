using UnityEngine;

public class BigBossEnemyUnit : EnemyUnit

{
    // âªÇì preview áºº¡Ò¡ºÒ·
    public override void ShowAttackPreview()
    {
        var grid = GridManager.Instance;
        Vector2Int center = grid.WorldToGrid(transform.position);

        Vector2Int[] dirs = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var d in dirs)
        {
            for (int i = 1; i <= attackRange; i++)
            {
                Vector2Int cell = center + d * i;

                if (!grid.InBounds(cell)) break;

                grid.HighlightEnemyPreview(cell);
            }
        }
    }

    // â¨ÁµÕáºº¡Ò¡ºÒ·
    public override void ExecutePlannedAttack()
    {
        var grid = GridManager.Instance;
        Vector2Int center = grid.WorldToGrid(transform.position);

        Vector2Int[] dirs = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var d in dirs)
        {
            for (int i = 1; i <= attackRange; i++)
            {
                Vector2Int cell = center + d * i;

                if (!grid.InBounds(cell)) break;

                if (grid.occupiedTiles.TryGetValue(cell, out Unit unit))
                {
                    if (unit is PlayerUnit pu)
                    {
                        pu.TakeDamage(attackDamage);
                        Debug.Log($"{name} hits {pu.name} at {cell}!");
                    }
                }
            }
        }
    }
}
