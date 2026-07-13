using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] float speed = 8f;

    float lifeTime;
    float timer;
    int damage;

    public PoolType PoolType => PoolType.Arrow;

    public GameObject GameObject => gameObject;

    Rigidbody2D rb;
    void Awake()
    {
        lifeTime = 3f;
        timer = 0f;
        rb = GetComponent<Rigidbody2D>();
        damage = 3;
    }

    private void OnEnable()
    {
        timer = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (timer > lifeTime)
        {
            ObjectPool.instance.ReturnObject(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            ObjectPool.instance.ReturnObject(this);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            ObjectPool.instance.ReturnObject(this);
            collision.gameObject.GetComponent<EnemyBase>().TakeDamage(damage);
        }
    }

    public void Init(float newDamage, float newSpeed)
    {
        damage = (int)newDamage;
        speed = newSpeed;
        rb.linearVelocity = speed * transform.right;
    }
}
