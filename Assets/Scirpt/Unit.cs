using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int attackDamage = 15;
    public float moveSpeed = 5f;
    public bool hasTakenAction = false;

    protected Vector2Int gridPos;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        // จองช่องที่ยืนตอนเกิด
        gridPos = GridManager.Instance.WorldToGrid(transform.position);
        GridManager.Instance.OccupyTile(gridPos, this);
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health is now {currentHealth}.");
        if (currentHealth <= 0) Die();
    }

    public virtual void ResetTurn() => hasTakenAction = false;

    protected virtual void Die()
    {
        // ปล่อยช่อง
        Vector2Int g = GridManager.Instance.WorldToGrid(transform.position);
        GridManager.Instance.FreeTile(g);

        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
