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
        nowHp -= damage;
        CheckHp();
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
