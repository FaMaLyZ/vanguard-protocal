using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int attackDamage = 15;
    public float moveSpeed = 5f;
    public bool hasTakenAction = false;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health is now {currentHealth}.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Reset the unit's state at the beginning of their turn
    public virtual void ResetTurn()
    {
        hasTakenAction = false;
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}