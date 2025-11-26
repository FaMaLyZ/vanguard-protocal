// EnemyUnit.cs
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit
{
    public float attackRange = 1f;
    public CharacterMovement characterMovement;

    private Vector2Int plannedAttackCell;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void Start()
    {
        GameManager.Instance?.RegisterEnemyUnit(this);
    }

    private void OnDestroy()
    {
        if (GridManager.Instance != null)
        {
            // ล้าง preview ทั้งหมด
            GridManager.Instance.ClearEnemyPreview();
        }

        // เอาออกจากระบบ
        GameManager.Instance?.RemoveEnemyUnit(this);

        // ให้ศัตรูที่ยังไม่ตายวาด preview ใหม่
        foreach (var enemy in GameManager.Instance.GetAllEnemies())
        {
            enemy.ShowAttackPreview();
        }
    }


    public void DoMovePhase()
    {
        PlayerUnit target = GameManager.Instance.GetClosestPlayerUnit(transform.position);

        if (target == null) return;

        MoveTowards(target);
    }
    public void ShowAttackPreview()
    {
        var grid = GridManager.Instance;
        Vector2Int myGrid = grid.WorldToGrid(transform.position);

        PlayerUnit target = GameManager.Instance.GetClosestPlayerUnit(transform.position);
        if (target == null) return;

        Vector2Int targetGrid = grid.WorldToGrid(target.transform.position);

        Vector2Int dir;

        int dx = targetGrid.x - myGrid.x;
        int dy = targetGrid.y - myGrid.y;

        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            dir = (dx > 0) ? Vector2Int.right : Vector2Int.left;
        else
            dir = (dy > 0) ? Vector2Int.up : Vector2Int.down;

        plannedAttackCell = myGrid + dir;

        grid.HighlightEnemyPreview(plannedAttackCell);
    }

    public void ExecutePlannedAttack()
    {
        var grid = GridManager.Instance;

        if (grid.occupiedTiles.TryGetValue(plannedAttackCell, out Unit unit))
        {
            if (unit is PlayerUnit pu)
            {
                pu.TakeDamage(attackDamage);
                Debug.Log($"{name} hits {pu.name}!");
            }
        }
    }

    public void ClearAttackPreview()
    {
        GridManager.Instance.ClearHighlights();
    }


    private void MoveTowards(PlayerUnit target)
    {
        var grid = GridManager.Instance;
        Vector2Int enemyGrid = grid.WorldToGrid(transform.position);
        Vector2Int targetGrid = grid.WorldToGrid(target.transform.position);

        // หาช่องว่างรอบเป้าหมายถ้าช่องเป้าหมายถูกยึด
        Vector2Int destGrid = targetGrid;
        if (!grid.IsTileFree(destGrid))
        {
            if (!grid.TryFindNearestFreeTile(targetGrid, enemyGrid, 3, out destGrid))
            {
                Debug.Log($"{name}: No reachable tile.");
                return;
            }
        }

        // หาเส้นทางด้วย BFS
        List<Vector2Int> path = FindPath(enemyGrid, destGrid);

        if (path == null || path.Count == 0)
        {
            Debug.Log($"{name}: No path to target.");
            return;
        }

        // ✅ จำกัดการเดินสูงสุด 3 ช่อง
        int maxStep = Mathf.Min(3, path.Count);

        // nextStep คือเป้าหมายสุดท้ายของ "3 ช่องแรก"
        Vector2Int nextStep = path[maxStep - 1];

        characterMovement.MoveToGrid(nextStep);

        Debug.Log($"{name} moves up to {maxStep} tiles → {nextStep}");
    }

    List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        var grid = GridManager.Instance;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goal)
                break;

            foreach (var next in grid.GetNeighbors4(current))
            {
                // ห้ามเดินทับ
                if (!grid.IsTileFree(next) && next != goal)
                    continue;

                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        // reconstruct
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int cur = goal;

        // goal unreachable?
        if (!cameFrom.ContainsKey(goal))
            return path;

        while (cur != start)
        {
            path.Add(cur);
            cur = cameFrom[cur];
        }

        path.Reverse();
        return path;
    }
    private PlayerUnit GetAdjacentPlayer()
    {
        var grid = GridManager.Instance;
        Vector2Int myGrid = grid.WorldToGrid(transform.position);

        // 4 ทิศ
        Vector2Int[] dirs =
        {
        new Vector2Int( 1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int( 0, 1),
        new Vector2Int( 0,-1)
    };

        foreach (var d in dirs)
        {
            Vector2Int check = myGrid + d;

            if (!grid.InBounds(check)) continue;

            if (grid.occupiedTiles.TryGetValue(check, out Unit unit))
            {
                if (unit is PlayerUnit pu)
                    return pu;
            }
        }

        return null;
    }
    public void ForceMoveTo(Vector2Int newGridPos)
    {
        Vector3 newWorldPos = GridManager.Instance.GridToWorld(newGridPos);

        // update world
        transform.position = newWorldPos;

        // update gridManager tile states
        GridManager.Instance.FreeTile(gridPos);   // old
        gridPos = newGridPos;                     // update protected field
        GridManager.Instance.OccupyTile(newGridPos, this);
    }
    public Vector2Int GetGridPos()
    {
        return gridPos;
    }

}
