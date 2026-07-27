using UnityEngine;
using UnityEngine.UIElements;

public class ExplosionEffect : IProjectileEffect
{
    private readonly float radius;
    private readonly float damage;
    private readonly PoolType effectPoolType = PoolType.ExplosionEffect;

    public ExplosionEffect(
        float radius,
        float damage,
        PoolType effectPoolType)
    {
        this.radius = radius;
        this.damage = damage;
        this.effectPoolType = effectPoolType;
    }

    public bool OnHit(Bullet bullet, EnemyBase target)
    {
        Vector3 explosionPosition = target.transform.position;

        PlayEffect(explosionPosition);
        ApplyExplosionDamage(explosionPosition, target);

        return false;
    }

    private void PlayEffect(Vector3 position)
    {
        EffectObject effect =
            ObjectPool.instance.GetObject<EffectObject>(
                effectPoolType
            );

        if (effect == null)
        {

            return;
        }

        effect.Play(position, radius);
    }

    private void ApplyExplosionDamage(
        Vector3 position,
        EnemyBase directTarget)
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(
                position,
                radius,
                LayerMask.GetMask("Enemy")
            );

        foreach (Collider2D enemyCollider in enemies)
        {
            EnemyBase enemy =
                enemyCollider.GetComponent<EnemyBase>();

            if (enemy == null)
            {
                continue;
            }

            // 직접 맞은 적은 Bullet에서 기본 피해를 받았으므로 제외
            if (enemy == directTarget)
            {
                continue;
            }

            enemy.TakeDamage(damage);
        }
    }
}