using UnityEngine;

public class Arrow : MonoBehaviour, IPoolable
{
    [SerializeField] float speed = 8f;

    float lifeTime;
    float timer;
    int damage;

    public PoolType PoolType => PoolType.Arrow;

    public GameObject GameObject => gameObject;

    Rigidbody2D rb;

    private void OnEnable()
    {
        timer = 0f;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lifeTime = 3f;
        timer = 0f;
        rb = GetComponent<Rigidbody2D>();
        damage = 3;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        rb.linearVelocity = speed*transform.right;

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
}
