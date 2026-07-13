using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterMoveController : MonoBehaviour
{
    [SerializeField] PoolType bulletType;

    private MonsterBulletAttackTouch monsterAttack;

    public Transform player;

    EnemyBase enemyBase;

    public bool isTrace {  get; private set; }


    Rigidbody2D rb;

    SpriteRenderer sr;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        enemyBase = gameObject.GetComponent<EnemyBase>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
    }

    //특정 거리보다 가까우면 추적 아니면 공격
    void CheckDistance()
    {
        float distance;
        distance = Vector2.Distance(transform.position, player.position);
        isTrace = distance > enemyBase.GetAttackDistance();
        //공격거리보다 멀면 따라간다.
        if(isTrace)
        {
            Trace();
        }
        else
        {
            //공격시 행동
            RangedAttack();
        }        
    }

    void Trace()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, enemyBase.GetMoveSpeed() * Time.deltaTime);
    }

    public virtual void RangedAttack()
    {
        if (player == null)
        {
            return;
        }

        if(bulletType == PoolType.None)
        {
            return;
        }

        monsterAttack = ObjectPool.instance.GetObject<MonsterBulletAttackTouch>(bulletType);

        if (monsterAttack == null)
        {
            Debug.LogError(
                $"{monsterAttack} 오브젝트에 MonsterBulletAttackTouch 컴포넌트가 없습니다.");
            return;
        }


        Vector2 dir = player.position - transform.position;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        monsterAttack.transform.position = transform.position;
        monsterAttack.transform.rotation = Quaternion.Euler(0f, 0f, baseAngle);

        monsterAttack.Init(
            enemyBase.GetDamage(),
            enemyBase.GetProjectileSpeed()
        );
    }
}
