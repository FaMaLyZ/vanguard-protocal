// EnemyUnit.cs
using UnityEngine;

public class EnemyUnit : Unit
{
    public float attackRange = 1f;
    private CharacterMovement characterMovement;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }
    private void Start()

    {
        if (GameManager.Instance !=null)
        {
            GameManager.Instance.RegisterEnemyUnit(this);
        }
    }
    void OnDestroy()
    {
        // Tell the GameManager that this unit has been removed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemoveEnemyUnit(this);
        }
    }

    /// <summary>
    /// This contains the AI logic for the enemy's turn.
    /// </summary>
    public void TakeTurn()
    {
        if (hasTakenAction) return;

        // Find the closest player unit to attack
        PlayerUnit target = GameManager.Instance.GetClosestPlayerUnit(transform.position);

        if (target == null)
        {
            Debug.Log($"{gameObject.name} has no targets left.");
            hasTakenAction = true;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        // 1. Check if target is in attack range
        if (distanceToTarget <= attackRange)
        {
            Attack(target);
            
        }
        // 2. If not, move towards the target
        else
        {
            MoveTowards(target);
        }

        hasTakenAction = true;
    }

    private void Attack(PlayerUnit target)
    {
        Debug.Log($"{gameObject.name} attacks {target.name}!");
        target.TakeDamage(attackDamage);
    }

    private void MoveTowards(PlayerUnit target)
    {
        Vector3 current = transform.position;
        Vector3 targetPos = target.transform.position;

        // ทำงานบนระนาบ XZ เท่านั้น
        Vector2 cur2 = new Vector2(current.x, current.z);
        Vector2 tgt2 = new Vector2(targetPos.x, targetPos.z);

        float distance = Vector2.Distance(cur2, tgt2);

        // ✅ เดินไม่เกิน 3 tile
        float maxStep = 3f;

        // ถ้าห่างน้อยกว่า 3 ก็เดินถึงได้เลย
        if (distance <= maxStep)
        {
            // Snap จุดปลายทางให้ตรงกลาง tile
            Vector3 snapped = new Vector3(Mathf.Round(targetPos.x), current.y, Mathf.Round(targetPos.z));
            characterMovement.MoveToDestination(snapped);
        }
        else
        {
            // ✅ ถ้าห่างเกิน 3 → เดินไปในทิศทาง target แต่ระยะ = 3 tile
            Vector2 dir = (tgt2 - cur2).normalized;    // ทิศทางไปหาเป้าหมาย
            Vector2 step = cur2 + dir * maxStep;

            // Snap ตำแหน่งเป้าหมายใหม่ให้อยู่ตรงกลาง tile
            Vector3 newPos = new Vector3(Mathf.Round(step.x), current.y, Mathf.Round(step.y));

            characterMovement.MoveToDestination(newPos);
        }

        Debug.Log($"{name} moves up to 3 tiles toward {target.name}");
    }

}