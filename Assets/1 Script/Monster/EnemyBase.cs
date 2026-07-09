using UnityEngine;

public class EnemyBase : CharacterStatus, IPoolable
{
    [SerializeField] protected PoolType poolType = PoolType.None;

    public PoolType PoolType => poolType;
    public GameObject GameObject => gameObject;

    protected virtual void OnEnable()
    {
        // 풀에서 다시 꺼내질 때 체력 초기화
        nowHp = maxHp;
    }


    protected override void Die()
    {
        ObjectPool.instance.ReturnObject(this);
    }
}