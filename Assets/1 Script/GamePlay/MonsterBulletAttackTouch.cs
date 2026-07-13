using UnityEngine;

public class MonsterBulletAttackTouch : AttackTouch, IPoolable
{
    float bulletSpeed;

    public PoolType PoolType => PoolType.None;

    public GameObject GameObject => gameObject;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ObjectPool.instance.ReturnObject(this);
        }
    }

    public void Init(float newDamage, float newSpeed)
    {
        SetDamage(newDamage);
        bulletSpeed = newSpeed;
    }
}
