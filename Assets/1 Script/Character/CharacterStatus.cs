using UnityEngine;

public abstract class CharacterStatus : MonoBehaviour
{
    [SerializeField] protected float maxHp;
    [SerializeField] protected float nowHp;
    public float MoveSpeed { get; protected set; }

    protected float takeDamageDelay;
    protected WaitForSeconds wait;

    protected bool isHit;

    public virtual void TakeDamage(float damage)
    {
#if UNITY_EDITOR
        Debug.Log(
       $"[Enemy] 받은 Damage: {damage}, 맞기 전 체력: {nowHp}");
#endif
        nowHp -= damage;
        CheckHp();
#if UNITY_EDITOR
        Debug.Log($"[Enemy] 맞은 후 체력: {nowHp}");
#endif
    }

    void CheckHp()
    {
        if(nowHp >= maxHp)
        {
            nowHp= maxHp;
        }
        if(nowHp<=0)
        {
            nowHp = 0;
            Die();
        }
    }

    protected abstract void Die();
}
