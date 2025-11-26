using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Unit _target;
    private int _damage;
    private float _speed = 15f;

    public System.Action<EnemyUnit> OnHitEnemy;   // สำหรับ Desert / Violet / Crimson

    public void Initialize(Unit target, int damage)
    {
        _target = target;
        _damage = damage;
    }

    void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (_target.transform.position - transform.position);
        float step = _speed * Time.deltaTime;

        // --------------------------
        // ตรวจ obstacle (ไม่ทำ damage)
        // --------------------------
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir.normalized, out hit, step + 0.01f))
        {
            if (hit.collider.GetComponent<Obstacle>() != null)
            {
                Debug.Log("Projectile hit obstacle and is destroyed.");
                Destroy(gameObject);
                return;
            }
        }

        // --------------------------
        // เคลื่อน projectile
        // --------------------------
        transform.position += dir.normalized * step;

        // --------------------------
        // ชนเป้าหมายจริง (impact frame)
        // --------------------------
        if (Vector3.Distance(transform.position, _target.transform.position) <= 0.3f)
        {
            // เรียก effect ก่อน damage
            OnHitEnemy?.Invoke(_target as EnemyUnit);

            // damage หลังชน
            _target.TakeDamage(_damage);

            Destroy(gameObject);
        }
    }
}
