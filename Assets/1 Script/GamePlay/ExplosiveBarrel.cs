using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

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

    [Header("폭발 지연")]
    [SerializeField] private float fuseDuration = 0.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    private bool isIgnited;

    [SerializeField]
    private Vector2 fuseDurationRange =
    new Vector2(0.25f, 0.55f);

    [Header("폭발 넉백")]
    [SerializeField]
    private float explosionKnockbackForce = 8f;

    [Header("플레이어 피해")]
    [SerializeField]
    private bool canDamagePlayer = true;

    [SerializeField]
    private float playerExplosionDamage = 20f;

    [SerializeField]
    private LayerMask playerLayer;

    [Header("폭발 범위 표시")]
    [SerializeField]
    private GameObject explosionRangeObject;

    [SerializeField]
    private SpriteRenderer explosionRangeRenderer;

    [Header("풀링")]
    [SerializeField]
    private PoolType poolType = PoolType.ExplosiveBarrel;

    public event Action<ExplosiveBarrel> OnReturnedToPool;

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

        SetupExplosionRange();
    }

    private void OnEnable()
    {
        currentHp = maxHp;

        isIgnited = false;
        isExploded = false;

        transform.localScale =
            Vector3.one;

        if (spriteRenderer != null)
        {
            spriteRenderer.color =
                Color.white;
        }

        if (explosionRangeObject != null)
        {
            explosionRangeObject.SetActive(false);

            SetupExplosionRange();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isExploded || isIgnited)
        {
            return;
        }

        currentHp -= damage;

        PlayHitFeedback();

        if (currentHp <= 0f)
        {
            Ignite();
        }
    }

    private void Ignite()
    {
        if (isIgnited || isExploded)
        {
            return;
        }

        isIgnited = true;

        if (explosionRangeObject != null)
        {
            explosionRangeObject.SetActive(true);
        }

        StartCoroutine(FuseRoutine());
    }

    private IEnumerator FuseRoutine()
    {
        float timer = 0f;
        bool isRed = false;

        Vector3 originalBarrelScale =
            transform.localScale;

        Vector3 originalRangeScale =
            explosionRangeObject != null
                ? explosionRangeObject.transform.localScale
                : Vector3.one;

        while (timer < fuseDuration)
        {
            timer += blinkInterval;

            float progress = Mathf.Clamp01(
                timer / fuseDuration
            );

            isRed = !isRed;

            if (spriteRenderer != null)
            {
                spriteRenderer.color =
                    isRed
                        ? Color.red
                        : Color.white;
            }

            transform.localScale =
                originalBarrelScale *
                Mathf.Lerp(
                    1f,
                    1.15f,
                    progress
                );

            if (explosionRangeRenderer != null)
            {
                Color rangeColor =
                    explosionRangeRenderer.color;

                rangeColor.a =
                    Mathf.Lerp(
                        0.2f,
                        0.7f,
                        progress
                    );

                explosionRangeRenderer.color =
                    rangeColor;
            }

            yield return new WaitForSeconds(
                blinkInterval
            );
        }

        transform.localScale =
            originalBarrelScale;

        if (explosionRangeObject != null)
        {
            explosionRangeObject.transform.localScale =
                originalRangeScale;
        }

        Explode();
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

        if (explosionRangeObject != null)
        {
            explosionRangeObject.SetActive(false);
        }

        DamageNearbyEnemies();
        DamageNearbyBarrels();
        PlayExplosionEffect();

        DamagePlayer();

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

            Vector2 direction =
            enemy.transform.position -
            transform.position;

            if (direction.sqrMagnitude < 0.01f)
            {
                direction = UnityEngine.Random.insideUnitCircle.normalized;
            }
            else
            {
                direction.Normalize();
            }

            enemy.ApplyKnockback(
                direction,
                explosionKnockbackForce
            );
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

    private void DamagePlayer()
    {
        if (!canDamagePlayer)
        {
            return;
        }

        Collider2D playerCollider =
            Physics2D.OverlapCircle(
                transform.position,
                explosionRadius,
                playerLayer
            );

        if (playerCollider == null)
        {
            return;
        }

        PlayerStatus player =
            playerCollider.GetComponentInParent<PlayerStatus>();

        if (player == null)
        {
            return;
        }

        player.TakeDamage(
            playerExplosionDamage
        );
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

    private void SetupExplosionRange()
    {
        if (explosionRangeObject == null)
        {
            return;
        }

        float diameter = explosionRadius * 2f;

        explosionRangeObject.transform.localScale =
            new Vector3(
                diameter,
                diameter,
                1f
            );
    }

    private void ReturnToPool()
    {
        OnReturnedToPool?.Invoke(this);

        // 다음 사용자를 위해 기존 구독을 모두 제거
        OnReturnedToPool = null;

        transform.SetParent(null);

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
        StopAllCoroutines();
        CancelInvoke();

        isIgnited = false;
        isExploded = false;

        transform.localScale =
            Vector3.one;

        if (spriteRenderer != null)
        {
            spriteRenderer.color =
                Color.white;
        }

        if (explosionRangeObject != null)
        {
            explosionRangeObject.SetActive(false);
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