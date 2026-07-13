using UnityEngine;

public class MonsterMoveController : MonoBehaviour
{
    [SerializeField] private PoolType bulletType;

    private MonsterBulletAttackTouch monsterAttack;

    public Transform player;

    private EnemyBase enemyBase;

    public bool isTrace { get; private set; }

    // 마지막 공격 이후 지난 시간
    private float attackTimer;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        enemyBase = GetComponent<EnemyBase>();
    }

    private void OnEnable()
    {
        // 풀에서 다시 나왔을 때 바로 공격하지 않게 초기화
        attackTimer = 0f;
    }

    private void Update()
    {
        // 매 프레임 공격 대기시간 증가
        attackTimer += Time.deltaTime;

        CheckDistance();
    }

    private void CheckDistance()
    {
        if (player == null)
        {
            return;
        }

        float distance =
            Vector2.Distance(transform.position, player.position);

        isTrace =
            distance > enemyBase.GetAttackDistance();

        if (isTrace)
        {
            Trace();
        }
        else
        {
            TryRangedAttack();
        }
    }

    private void Trace()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            enemyBase.GetMoveSpeed() * Time.deltaTime
        );
    }

    // 공격 가능 시간이 되었는지 확인
    private void TryRangedAttack()
    {
        if (attackTimer < enemyBase.GetAttackDelay())
        {
            return;
        }

        RangedAttack();

        // 공격 후 타이머 초기화
        attackTimer = 0f;
    }

    public virtual void RangedAttack()
    {
        if (player == null)
        {
            return;
        }

        if (bulletType == PoolType.None)
        {
            return;
        }

        monsterAttack = ObjectPool.instance.GetObject<MonsterBulletAttackTouch>(bulletType);

        if (monsterAttack == null)
        {
            Debug.LogError(
                $"{bulletType} 프리팹에 " +
                "MonsterBulletAttackTouch 컴포넌트가 없습니다.");
            return;
        }

        Vector2 direction =
            player.position - transform.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        monsterAttack.transform.SetPositionAndRotation(
            transform.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        monsterAttack.Init(
            enemyBase.GetDamage(),
            enemyBase.GetProjectileSpeed()
        );
    }
}