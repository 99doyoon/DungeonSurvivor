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
    }

    protected override void Die()
    {
        ObjectPool.instance.ReturnObject(this);
    }

    public float GetMoveSpeed() => monsterData.moveSpeed;
    public float GetDamage() => monsterData.damage;
    public float GetAttackDistance() => monsterData.attackDistance;
    public float GetMaxHp() => monsterData.maxHp;
    public MonsterData GetMonsterData() => monsterData;
}