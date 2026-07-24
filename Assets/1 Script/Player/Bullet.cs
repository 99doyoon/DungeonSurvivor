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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ExplosiveBarrel barrel =
        collision.GetComponentInParent<
            ExplosiveBarrel
        >();

        if (barrel != null)
        {
            barrel.TakeDamage(damage);

            ObjectPool.instance.ReturnObject(this);
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            ObjectPool.instance.ReturnObject(this);
            return;
        }

        if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            return;
        }

        EnemyBase enemy =
            collision.GetComponentInParent<EnemyBase>();

        if (enemy == null)
        {
            return;
        }

        enemy.TakeDamage(damage);

        bool keepFlying = false;

        foreach (IProjectileEffect effect in effects)
        {
            if (effect.OnHit(this, enemy))
            {
                keepFlying = true;
            }
        }

        if (!keepFlying)
        {
            ObjectPool.instance.ReturnObject(this);
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
        this.speed = newSpeed;

        effects.Clear();

        if (newEffects != null)
        {
            effects.AddRange(newEffects);
        }

        gameObject.SetActive(true);
        rb.linearVelocity = speed * transform.right;
    }

    //불렛이 방향을 바꾸기위한 함수
    public void ChangeDirection(Vector2 dir)
    {
        dir.Normalize();

        float angle =
            Mathf.Atan2(dir.y, dir.x) *
            Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        // 현재 Arrow의 이동 방식에 맞춰 속도도 갱신
        rb.linearVelocity = dir.normalized * speed;

        transform.position += (Vector3)(dir * 0.05f);
    }
}
