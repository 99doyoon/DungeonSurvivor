using System;
using UnityEngine;
using System.Collections;

public class EnemyBase : CharacterStatus, IPoolable
{
    [Header("몬스터 데이터")]
    [SerializeField] private MonsterData monsterData;

    MonsterAnimation monsterAnimation;

    public MonsterData Data => monsterData;
    public float CurrentHp => nowHp;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float knockbackDuration = 0.15f;

    private bool isKnockback;

    public PoolType PoolType =>
        monsterData != null
            ? monsterData.poolType
            : PoolType.None;

    public GameObject GameObject => gameObject;

    /// <summary>
    /// 이 이벤트를 구독한 컴포넌트가 있으면
    /// EnemyBase가 바로 풀에 반환하지 않고 사망 처리를 맡긴다.
    /// </summary>
    public event Action<EnemyBase> OnDeathRequested;

    public bool isDead { get; private set; }

    public bool IsKnockback => isKnockback;

    public event Action<float, float> OnHpChanged;
    public event Action<EnemyBase> OnInitialized;

    protected virtual void OnEnable()
    {
        Init();
    }

    public virtual void Init()
    {
        isDead = false;

        if (monsterData == null)
        {
            Debug.LogError(
                $"{gameObject.name}의 MonsterData가 연결되지 않았습니다.",
                gameObject
            );

            return;
        }

        maxHp = monsterData.maxHp;
        nowHp = maxHp;

        // AttackTouch가 있는 몬스터만 공격력을 설정한다.
        if (TryGetComponent(out AttackTouch attackTouch))
        {
            attackTouch.SetDamage(monsterData.damage);
        }
        else
        {
            Debug.LogWarning(
                $"{gameObject.name}에 AttackTouch 컴포넌트가 없습니다.",
                gameObject
            );
        }

        monsterAnimation= GetComponentInChildren<MonsterAnimation>();

        OnInitialized?.Invoke(this);

        OnHpChanged?.Invoke(nowHp, maxHp);

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    /// <summary>
    /// CharacterStatus에서 체력이 0 이하가 되었을 때 호출된다.
    /// </summary>
    protected override void Die()
    {
        // 중복 사망 처리 방지
        if (isDead)
            return;

        isDead = true;
        nowHp = 0f;

        //gameover시 킬을 보여주기위해
        GameResultManager.Instance?.AddKill();

        //몬스터 사망 사운드 재생
        SoundManager.Instance?.PlaySfx(SFXType.EnemyDie);

        /*
         * BossMonster처럼 사망 이벤트를 구독한 컴포넌트가 있다면
         * 죽음 애니메이션과 풀 반환을 해당 컴포넌트에 맡긴다.
         */
        if (OnDeathRequested != null)
        {
            OnDeathRequested.Invoke(this);
            return;
        }

        // 별도의 사망 연출이 없는 일반 몬스터는 즉시 처리
        CompleteDeath();
    }

    /// <summary>
    /// 경험치를 생성하고 몬스터를 풀에 반환한다.
    /// 보스는 죽음 애니메이션이 끝난 뒤 이 함수를 호출하면 된다.
    /// </summary>
    public void CompleteDeath()
    {
#if UNITY_EDITOR
        Debug.Log($"CompleteDeath 호출: {name}", gameObject);
#endif
        PlayDeathEffect();

        DropExp();

        ObjectPool.instance.ReturnObject(this);
    }

    /// <summary>
    /// 경험치 아이템을 생성한다.
    /// </summary>
    protected virtual void DropExp()
    {
        GameObject expItem =
            ObjectPool.instance.GetObject(PoolType.ExpItem);

        if (expItem == null)
        {
            Debug.LogWarning(
                "ExpItem을 오브젝트 풀에서 가져오지 못했습니다."
            );

            return;
        }

        if (!expItem.TryGetComponent(out ExpItem exp))
        {
            Debug.LogError(
                $"{expItem.name}에 ExpItem 컴포넌트가 없습니다.",
                expItem
            );

            IPoolable poolable =
                expItem.GetComponent<IPoolable>();

            if (poolable != null)
            {
                ObjectPool.instance.ReturnObject(poolable);
            }

            return;
        }

        exp.SetExp(GetExp());
        expItem.transform.position = transform.position;
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        OnHpChanged?.Invoke(nowHp, maxHp);

        if (monsterAnimation != null)
        {
            monsterAnimation.HitAnimation();
        }

#if UNITY_EDITOR
        Debug.Log($"피해 발생: {damage}");
#endif

        ShowDamageText(damage);

        //몬스터 피격시 사운드재생
        if (nowHp > 0)
        {
            SoundManager.Instance?.PlaySfx(
                SFXType.EnemyHit
            );
        }
    }

    private void ShowDamageText(float damage)
    {
#if UNITY_EDITOR
        Debug.Log($"ShowDamageText 호출: {damage}");
#endif

        DamageText text =
            ObjectPool.instance.GetObject<DamageText>(
                PoolType.DamageText
            );

        if (text == null)
        {
#if UNITY_EDITOR
            Debug.LogError("DamageText를 풀에서 가져오지 못했습니다.");
#endif
            return;
        }

        Vector3 spawnPosition =
            transform.position + Vector3.up;
#if UNITY_EDITOR
        Debug.Log($"DamageText 생성 위치: {spawnPosition}");
#endif
        spawnPosition.z = -1f;
        text.Play(spawnPosition, damage);
    }

    private void PlayDeathEffect()
    {
#if UNITY_EDITOR
        Debug.Log(
            $"사망 이펙트 요청: {name}, 위치: {transform.position}",
            gameObject
        );
#endif
        EnemyDeathEffect effect =
            ObjectPool.instance.GetObject<EnemyDeathEffect>(
                PoolType.EnemyDeathEffect
            );

        if (effect == null)
        {
            return;
        }
#if UNITY_EDITOR
        Debug.Log(
           $"사망 이펙트 가져오기 성공: {effect.name}",
           effect.gameObject
       );
#endif
        SpriteRenderer spriteRenderer =
            GetComponentInChildren<SpriteRenderer>();

        Color effectColor =
            spriteRenderer != null
            ? spriteRenderer.color
            : Color.white;

        effect.Play(
            transform.position,
            effectColor
        );
    }

    public void ClearDeathRequest()
    {
        OnDeathRequested = null;
    }

    public string GetMonsterName()
    {
        if (monsterData == null)
        {
            return gameObject.name.Replace(
                "(Clone)",
                ""
            );
        }

        if (string.IsNullOrWhiteSpace(
            monsterData.monsterName))
        {
            return gameObject.name.Replace(
                "(Clone)",
                ""
            );
        }

        return monsterData.monsterName;
    }

    public void ApplyKnockback(
    Vector2 direction,
    float force)
    {
        if (rb == null)
        {
            return;
        }

        StartCoroutine(
            KnockbackRoutine(direction, force)
        );
    }

    private IEnumerator KnockbackRoutine(
    Vector2 direction,
    float force)
    {
        isKnockback = true;

        rb.linearVelocity =
            direction.normalized * force;

        yield return new WaitForSeconds(
            knockbackDuration
        );

        rb.linearVelocity = Vector2.zero;

        isKnockback = false;
    }

    public int GetExp() =>
        monsterData != null ? monsterData.Exp : 0;

    public float GetMoveSpeed() =>
        monsterData != null ? monsterData.moveSpeed : 0f;

    public float GetDamage() =>
        monsterData != null ? monsterData.damage : 0f;

    public float GetAttackDistance() =>
        monsterData != null ? monsterData.attackDistance : 0f;

    public float GetMaxHp() =>
        monsterData != null ? monsterData.maxHp : 0f;

    public float GetProjectileSpeed() =>
        monsterData != null ? monsterData.projectileSpeed : 0f;

    public float GetAttackDelay() =>
        monsterData != null ? monsterData.attackDelay : 0f;

    public MonsterData GetMonsterData() => monsterData;
}