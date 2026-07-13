using Unity.Burst.Intrinsics;
using UnityEngine;

public class EnemyBase : CharacterStatus, IPoolable
{
    [Header("몬스터 데이터")]
    [SerializeField] private MonsterData monsterData;

    public MonsterData Data => monsterData;
    public float CurrentHp => nowHp;

    public PoolType PoolType => monsterData != null ? monsterData.poolType : PoolType.None;
    public GameObject GameObject => gameObject;

    protected virtual void OnEnable()
    {
        Init();
    }

    public virtual void Init()
    {
        if (monsterData == null)
        {
            Debug.LogError($"{gameObject.name}의 MonsterData가 연결되지 않았습니다.");
            return;
        }
        maxHp = monsterData.maxHp;
        nowHp = maxHp;

        AttackTouch attackTouch = gameObject.GetComponent<AttackTouch>();
        if(attackTouch == null)
        {
            Debug.LogError($"{gameObject}에 AttackTouch컴포넌트가 없습니다.");
            return;
        }
        attackTouch.SetDamage(monsterData.damage);
    }

    protected override void Die()
    {
        GameObject expItem = ObjectPool.instance.GetObject(PoolType.ExpItem);
        ExpItem exp = expItem.GetComponent<ExpItem>();
        exp.SetExp(GetExp());

        expItem.transform.position = transform.position;

        ObjectPool.instance.ReturnObject(this);
    }

    public int GetExp() => monsterData.Exp;
    public float GetMoveSpeed() => monsterData.moveSpeed;
    public float GetDamage() => monsterData.damage;
    public float GetAttackDistance() => monsterData.attackDistance;
    public float GetMaxHp() => monsterData.maxHp;
    public MonsterData GetMonsterData() => monsterData;

    public float GetProjectileSpeed() => monsterData.projectileSpeed;
}