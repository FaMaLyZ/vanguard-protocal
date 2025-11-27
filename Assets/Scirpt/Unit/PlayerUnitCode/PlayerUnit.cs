// PlayerUnit.cs
using UnityEngine;

public class PlayerUnit : Unit
{
    [Header("Attack Settings")]
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
            Debug.LogError("Projectile prefab missing on " + name);
            return;
        }

        // ยิงกระสุน
        FireProjectile(target);

        hasTakenAction = true;
    }
    protected void FireProjectile(EnemyUnit target)
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        GameObject projGO = Instantiate(projectilePrefab, origin, Quaternion.identity);

        Projectile proj = projGO.GetComponent<Projectile>();
        proj.Initialize(target, this);
    }


    // เผื่อมีการเรียก move แบบ world จากที่อื่น
    public void Move(Vector3 worldDestination)
    {
        if (hasTakenAction || GameManager.Instance.CurrentState != GameState.PlayerTurn) return;
        Vector2Int grid = GridManager.Instance.WorldToGrid(worldDestination);
        characterMovement.MoveToGrid(grid);
        hasTakenAction = true;
    }
    public virtual void OnProjectileImpact(EnemyUnit target)
    {
        // default = ยิงธรรมดา 1 dmg
        target.TakeDamage(attackDamage);
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
