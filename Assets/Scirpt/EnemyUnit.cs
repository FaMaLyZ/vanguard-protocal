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
        // ---> THIS IS WHERE YOUR ENEMY MOVEMENT LOGIC WOULD GO <---
        if (characterMovement != null)
        {
            characterMovement.MoveToDestination(target.transform.position);
        }
        else
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime; // Simple move logic
            Debug.LogWarning("CharacterMovement script not found on " + name + ". Moving manually.");
            
        }
        Debug.Log($"{gameObject.name} moves towards {target.name}.");
    }
}