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
    public float Damage { get; private set; } = 10f;

    [field: SerializeField]
    public float AttackDelay { get; private set; } = 1f;

    [field: SerializeField]
    public int ProjectileCount { get; private set; } = 1;

    [field: SerializeField]
    public float ProjectileSpeed { get; private set; } = 5f;

    [field: SerializeField]
    public int HealingAmount { get; private set; } = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveSpeed = 5;
        base.maxHp = 100;
        base.nowHp = base.maxHp;
        Level = 1;
        CurrentExp = 0;

        playerAnimationController = GetComponentInChildren<PlayerAnimationController>();

        isHit = true;
        takeDamageDelay = 1;
        wait = new WaitForSeconds(takeDamageDelay);
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

    public void Heal(int value)
    {
        nowHp += value;
        nowHp = Mathf.Min(nowHp, maxHp);
    }

    public void AddProjectile(int value)
    {
        ProjectileCount += value;
    }

    public void IncreaseProjectileSpeed(float value)
    {
        ProjectileSpeed += value;
    }
}
