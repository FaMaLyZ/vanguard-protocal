using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;

    private EnemyUnit target;
    private PlayerUnit owner;

    public void Initialize(EnemyUnit target, PlayerUnit owner)
    {
        this.target = target;
        this.owner = owner;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
        {
            OnHitTarget();
        }
    }

    private void OnHitTarget()
    {
        if (owner != null && target != null)
            owner.OnProjectileImpact(target);

        Destroy(gameObject);
    }
}
