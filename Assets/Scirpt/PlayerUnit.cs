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

    public void Attack(EnemyUnit target)
    {
        if (hasTakenAction || GameManager.Instance.CurrentState != GameState.PlayerTurn)
        {
            Debug.Log("Cannot attack: Not your turn or you've already acted.");
            return;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is not assigned on " + name);
            return;
        }

        // origin slightly above unit to avoid ground hits
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        Vector3 targetPos = target.transform.position + Vector3.up * 1.0f;
        Vector3 dir = (targetPos - origin).normalized;
        float dist = Vector3.Distance(origin, targetPos);

        // Raycast and check for obstacle
        if (Physics.Raycast(origin, dir, out RaycastHit hitInfo, dist))
        {
            // If we hit something before reaching target
            if (hitInfo.collider != null)
            {
                // If the hit is the target's collider -> proceed
                if (hitInfo.collider.GetComponent<EnemyUnit>() == target)
                {
                    // ok to shoot
                }
                else if (hitInfo.collider.GetComponent<Obstacle>() != null)
                {
                    Debug.Log($"{name}'s shot blocked by obstacle at {GridManager.Instance.WorldToGrid(hitInfo.collider.transform.position)}");
                    // optionally: play blocked sound/feedback
                    return; // cancel attack
                }
                else
                {
                    // hit something else (e.g. another unit) - treat as blocked
                    Debug.Log($"{name}'s shot blocked by {hitInfo.collider.name}");
                    return;
                }
            }
        }

        // Spawn projectile
        Debug.Log($"{gameObject.name} fires at {target.name}!");
        GameObject projectileGO = Instantiate(projectilePrefab, transform.position + Vector3.up, Quaternion.identity);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(target, attackDamage);
        }

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
