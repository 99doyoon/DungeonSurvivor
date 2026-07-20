using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField] float speed = 8f;

    private readonly List<IProjectileEffect> effects
    = new List<IProjectileEffect>();

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
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

            foreach (IProjectileEffect effect in effects)
            {
                effect.OnHit(this, enemy);
            }

            ObjectPool.instance.ReturnObject(this);
            collision.gameObject.GetComponent<EnemyBase>().TakeDamage(damage);
        }
    }

    //일반화살에대한 init함수
    public void Init(float newDamage, float newSpeed)
    {
        damage = (int)newDamage;
        speed = newSpeed;
        rb.linearVelocity = speed * transform.right;
    }

    //추가효과를 적용하려고할때 쓰는 init함수
    public void Init(
    float damage,
    float newSpeed,
    List<IProjectileEffect> newEffects)
    {
        this.damage = (int)damage;

        effects.Clear();

        if (newEffects != null)
        {
            effects.AddRange(newEffects);
        }

        speed = newSpeed;
        rb.linearVelocity = speed * transform.right;
    }
}
