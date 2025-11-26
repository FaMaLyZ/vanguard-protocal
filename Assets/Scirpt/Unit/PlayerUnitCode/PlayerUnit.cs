// PlayerUnit.cs
using UnityEngine;

public class PlayerUnit : Unit
{
    [Header("Dependencies")]
    public GameObject projectilePrefab;
    public CharacterMovement characterMovement;
    public int movementRange = 3;
    public int attackRange = 3;


    protected override void Start()
    {
        base.Start();
        if (!characterMovement) characterMovement = GetComponent<CharacterMovement>();
        GameManager.Instance?.RegisterPlayerUnit(this);
    }

    private void OnDestroy()
    {
        GameManager.Instance?.RemovePlayerUnit(this);
    }

    public virtual void Attack(EnemyUnit target)
    {
        if (hasTakenAction || GameManager.Instance.CurrentState != GameState.PlayerTurn)
            return;

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab missing on " + name);
            return;
        }

        // ยิงปกติแบบ base
        FireProjectile(target);
        hasTakenAction = true;
    }
    protected void FireProjectile(EnemyUnit target)
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 targetPos = target.transform.position + Vector3.up * 1.0f;
        Vector3 dir = (targetPos - origin).normalized;
        float dist = Vector3.Distance(origin, targetPos);

        if (Physics.Raycast(origin, dir, out RaycastHit hitInfo, dist))
        {
            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.GetComponent<EnemyUnit>() != target)
                    return;
            }
        }

        GameObject projectileGO = Instantiate(projectilePrefab, origin, Quaternion.identity);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        projectile.Initialize(target, attackDamage);
    }


    // เผื่อมีการเรียก move แบบ world จากที่อื่น
    public void Move(Vector3 worldDestination)
    {
        if (hasTakenAction || GameManager.Instance.CurrentState != GameState.PlayerTurn) return;
        Vector2Int grid = GridManager.Instance.WorldToGrid(worldDestination);
        characterMovement.MoveToGrid(grid);
        hasTakenAction = true;
    }

    public void MoveToGrid(Vector2Int grid)
    {
        if (hasTakenAction || GameManager.Instance.CurrentState != GameState.PlayerTurn) return;
        characterMovement.MoveToGrid(grid);
        hasTakenAction = true;
    }
    protected Vector2Int GridPos(Vector3 world)
    {
        return new Vector2Int(
            Mathf.RoundToInt(world.x / GridManager.Instance.tileSize),
            Mathf.RoundToInt(world.z / GridManager.Instance.tileSize)
        );
    }

}
