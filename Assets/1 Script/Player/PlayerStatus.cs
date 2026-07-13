using System.Collections;
using UnityEngine;

public class PlayerStatus : CharacterStatus, IHit
{
    PlayerAnimationController playerAnimationController;

    [Header("레벨 및 경험치")]
    [field: SerializeField]
    public int Level { get; private set; }

    [field: SerializeField]
    public int CurrentExp { get; private set; }

    [Header("플레이어 능력치")]
    [field: SerializeField]
    public float Damage { get; private set; }

    [field: SerializeField]
    public float AttackDelay { get; private set; }

    [field: SerializeField]
    public int ProjectileCount { get; private set; }

    [field: SerializeField]
    public float ProjectileSpeed { get; private set; }

    [Header("자동 회복")]
    [field: SerializeField]
    public int AutoHealAmount { get; private set; }

    [field: SerializeField]
    public float AutoHealInterval { get; private set; }

    private Coroutine autoHealCoroutine;

    [field: SerializeField]
    public float AttackRange { get; private set; }

    void Start()
    {
        //MoveSpeed = 5;
        //base.maxHp = 100;
        //base.nowHp = base.maxHp;
        //Level = 1;
        //CurrentExp = 0;

        //playerAnimationController = GetComponentInChildren<PlayerAnimationController>();

        //isHit = true;
        //takeDamageDelay = 1;
        //wait = new WaitForSeconds(takeDamageDelay);

        StartGameStatusInit();
    }

    void StartGameStatusInit()
    {
        MoveSpeed = 5f;

        maxHp = 100;
        nowHp = maxHp;

        Level = 1;
        CurrentExp = 0;

        Damage = 5f;
        AttackDelay = 1f;
        ProjectileCount = 1;
        ProjectileSpeed = 5f;

        AutoHealAmount = 0;
        AutoHealInterval = 5f;

        playerAnimationController =
            GetComponentInChildren<PlayerAnimationController>();

        isHit = true;
        takeDamageDelay = 1f;
        wait = new WaitForSeconds(takeDamageDelay);

        StartAutoHeal();
    }

    protected override void Die()
    {
#if UNITY_EDITOR
        Debug.Log("Player Die");
#endif
    }
    public bool Hit()
    {
        if(isHit)
        {
            StartCoroutine(SetIsHit());
            playerAnimationController.HitAnimation();
            return true;
        }
        else
        {
            return false;
        }
    }

    //takeDamageDelay동안 데미지를 입지않기위해 isHit을 false로 만든다
    public IEnumerator SetIsHit()
    {
        isHit = false;
        yield return wait;
        isHit = true;
    }

    public void AddExp(int exp)
    {
        CurrentExp += exp;
    }

    public void AddLevel(int level)
    {
        Level += level;
    }

    public void SetExp(int exp)
    {
        CurrentExp = exp;
    }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void IncreaseDamage(float value)
    {
        Damage += value;

#if UNITY_EDITOR
        Debug.Log(
         $"[PlayerStatus] 증가량: {value}, 현재 Damage: {Damage}");
#endif
    }

    public void IncreaseAttackSpeed(float value)
    {
        AttackDelay -= value;
        AttackDelay = Mathf.Max(0.1f, AttackDelay);
    }

    public void IncreaseMoveSpeed(float value)
    {
        MoveSpeed += value;
    }

    public void IncreaseMaxHp(int value)
    {
        maxHp += value;
        nowHp += value;
    }


    public void AddProjectile(int value)
    {
        ProjectileCount += value;
#if UNITY_EDITOR
        Debug.Log(
        $"투사체 증가 완료 / 증가량: {value} / 현재 개수: {ProjectileCount}");
#endif
    }

    public void IncreaseProjectileSpeed(float value)
    {
        ProjectileSpeed += value;
    }
    private void StartAutoHeal()
    {
        if (autoHealCoroutine != null)
        {
            StopCoroutine(autoHealCoroutine);
        }

        autoHealCoroutine = StartCoroutine(AutoHealRoutine());
    }

    private IEnumerator AutoHealRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(AutoHealInterval);

            if (AutoHealAmount <= 0)
            {
                continue;
            }

            if (nowHp >= maxHp)
            {
                continue;
            }

            Heal(AutoHealAmount);
        }
    }

    public void Heal(int value)
    {
        nowHp = Mathf.Min(nowHp + value, maxHp);

#if UNITY_EDITOR
        Debug.Log($"자동 회복: {value}, 현재 체력: {nowHp}/{maxHp}");
#endif
    }

    public void IncreaseAutoHealAmount(int value)
    {
        AutoHealAmount += value;
    }

    public void DecreaseAutoHealInterval(float value)
    {
        AutoHealInterval -= value;
        AutoHealInterval = Mathf.Max(0.5f, AutoHealInterval);

        // WaitForSeconds가 기존 시간으로 만들어져 있을 수 있으므로 재시작
        StartAutoHeal();
    }
}
