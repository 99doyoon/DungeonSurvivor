using UnityEngine;

public class ExplosionEffect : IProjectileEffect
{
    private readonly float radius;
    private readonly float damage;

    public ExplosionEffect(float radius, float damage)
    {
        this.radius = radius;
        this.damage = damage;
    }

    public void OnHit(Bullet bullet, EnemyBase target)
    {
        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(target.transform.position, radius);

        foreach (Collider2D collider in colliders)
        {
            EnemyBase nearbyEnemy =
                collider.GetComponent<EnemyBase>();

            if (nearbyEnemy == null)
            {
                continue;
            }

            if (nearbyEnemy == target)
            {
                continue;
            }

            nearbyEnemy.TakeDamage(damage);
        }
    }
}