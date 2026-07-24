using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour, IPoolable
{
    [Header("배럴 체력")]
    [SerializeField] private float maxHp = 30f;

    [Header("폭발")]
    [SerializeField] private float explosionDamage = 50f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("참조")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("풀링")]
    [SerializeField]
    private PoolType poolType = PoolType.ExplosiveBarrel;

    private float currentHp;
    private bool isExploded;

    public PoolType PoolType => poolType;
    public GameObject GameObject => gameObject;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer =
                GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        currentHp = maxHp;
        isExploded = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isExploded)
        {
            return;
        }

        currentHp -= damage;

        PlayHitFeedback();

        if (currentHp <= 0f)
        {
            Explode();
        }
    }

    private void PlayHitFeedback()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.color = Color.red;
        CancelInvoke(nameof(ResetColor));
        Invoke(nameof(ResetColor), 0.1f);
    }

    private void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    private void Explode()
    {
        if (isExploded)
        {
            return;
        }

        isExploded = true;

        DamageNearbyEnemies();
        DamageNearbyBarrels();
        PlayExplosionEffect();

        SoundManager.Instance?.PlaySfx(
            SFXType.Explosion
        );

        CameraShake.Instance?.Play();

        ReturnToPool();
    }

    private void DamageNearbyEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius,
            enemyLayer
        );

        // 한 몬스터에게 Collider가 여러 개 있어도
        // 피해를 한 번만 주기 위해 HashSet을 사용한다.
        HashSet<EnemyBase> damagedEnemies =
            new HashSet<EnemyBase>();

        foreach (Collider2D hit in hits)
        {
            EnemyBase enemy =
                hit.GetComponentInParent<EnemyBase>();

            if (enemy == null)
            {
                continue;
            }

            if (!damagedEnemies.Add(enemy))
            {
                continue;
            }

            enemy.TakeDamage(explosionDamage);
        }
    }

    private void DamageNearbyBarrels()
    {
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );

        foreach (Collider2D hit in hits)
        {
            ExplosiveBarrel barrel =
                hit.GetComponentInParent<ExplosiveBarrel>();

            if (barrel == null ||
                barrel == this)
            {
                continue;
            }

            barrel.TakeDamage(
                explosionDamage
            );
        }
    }

    private void PlayExplosionEffect()
    {
        // 우선 기존 적 사망 이펙트를 재사용해도 된다.
        EnemyDeathEffect effect =
            ObjectPool.instance
                .GetObject<EnemyDeathEffect>(
                    PoolType.EnemyDeathEffect
                );

        if (effect == null)
        {
            return;
        }

        effect.Play(
            transform.position,
            new Color(1f, 0.4f, 0f, 1f)
        );
    }

    private void ReturnToPool()
    {
        if (ObjectPool.instance != null)
        {
            ObjectPool.instance.ReturnObject(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
#endif
}