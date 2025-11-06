// PlayerUnit.cs
using UnityEngine;

public class PlayerUnit : Unit
{
    [Header("Dependencies")]
    public GameObject projectilePrefab;
    public CharacterMovement characterMovement;

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

    public void Attack(EnemyUnit target)
    {
        if (hasTakenAction || GameManager.Instance.CurrentState != GameState.PlayerTurn) return;
        if (!projectilePrefab)
        {
            Debug.LogError("Projectile Prefab is not assigned on " + name);
            return;
        }

        GameObject projectileGO = Instantiate(projectilePrefab, transform.position + Vector3.up, Quaternion.identity);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null) projectile.Initialize(target, attackDamage);

        hasTakenAction = true;
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
}
