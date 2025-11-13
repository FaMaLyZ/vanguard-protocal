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
        if (_target == null) { Destroy(gameObject); return; }

        Vector3 dir = (_target.transform.position - transform.position).normalized;
        float step = _speed * Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, step + 0.01f))
        {
            if (hit.collider.GetComponent<Obstacle>() != null)
            {
                Debug.Log("Projectile hit obstacle and is destroyed.");
                Destroy(gameObject);
                return;
            }
            if (hit.collider.GetComponent<Unit>() != null && hit.collider.GetComponent<Unit>() == _target)
            {
                _target.TakeDamage(_damage);
                Destroy(gameObject);
                return;
            }
        }

        transform.position += dir * step;

        if (Vector3.Distance(transform.position, _target.transform.position) < 0.5f)
        {
            _target.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }

}