// Projectile.cs
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Unit _target;
    private int _damage;
    private float _speed = 15f;

    /// <summary>
    /// Call this immediately after instantiating the projectile.
    /// </summary>
    public void Initialize(Unit target, int damage)
    {
        _target = target;
        _damage = damage;
    }

    void Update()
    {
        if (_target == null)
        {
            // If the target died while the projectile was in the air
            Destroy(gameObject);
            return;
        }

        // Move towards the target
        Vector3 direction = (_target.transform.position - transform.position).normalized;
        transform.position += direction * _speed * Time.deltaTime;

        // Check if we have reached the target
        if (Vector3.Distance(transform.position, _target.transform.position) < 0.5f)
        {
            _target.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}