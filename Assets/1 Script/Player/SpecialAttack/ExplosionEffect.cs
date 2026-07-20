using UnityEngine;
using UnityEngine.UIElements;

public class ExplosionEffect : IProjectileEffect
{
    private readonly float radius;
    private readonly float damage;
    private readonly PoolType effectPoolType;

    public ExplosionEffect(
        float radius,
        float damage,
        PoolType effectPoolType)
    {
        this.radius = radius;
        this.damage = damage;
        this.effectPoolType = effectPoolType;
    }

    public void OnHit(Bullet bullet, EnemyBase target)
    {
        Vector3 explosionPosition = target.transform.position;

#if UNITY_EDITOR
        Debug.Log(
        $"폭발 효과 실행: 위치 {explosionPosition}, 피해량 {damage}"
    );
#endif

        PlayEffect(explosionPosition);
        ApplyExplosionDamage(explosionPosition, target);
    }

    private void PlayEffect(Vector3 position)
    {
        EffectObject effect =
            ObjectPool.instance.GetObject<EffectObject>(
                effectPoolType
            );

        if (effect == null)
        {
            Debug.LogWarning(
                $"{effectPoolType} 이펙트를 풀에서 가져오지 못했습니다."
            );

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