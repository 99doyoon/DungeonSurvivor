using UnityEngine;

public class MonsterBulletAttackTouch : AttackTouch, IPoolable
{
    float bulletSpeed;

    float lifeTime;
    float timer;

    Rigidbody2D rb;

    [Header("Pool 설정")]
    [SerializeField] private PoolType poolType;

    public PoolType PoolType => poolType;

    public GameObject GameObject => gameObject;

    void Awake()
    {
        lifeTime = 3f;
        timer = 0f;
        rb = GetComponent<Rigidbody2D>();
        damage = 3;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer > lifeTime)
        {
            ObjectPool.instance.ReturnObject(this);
        }
    }

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
        rb.linearVelocity = bulletSpeed * transform.right;
    }
}
