using UnityEngine;
using System.Collections;

public class MonsterMoveController : MonoBehaviour
{
    [SerializeField] private MonsterAnimation monsterAnimation;

    [SerializeField] private PoolType bulletType;

    private MonsterBulletAttackTouch monsterAttack;

    public Transform player;

    private EnemyBase enemyBase;

    private Collider2D monsterCollider;

    public bool isTrace { get; private set; }

    // 마지막 공격 이후 지난 시간
    private float attackTimer;

    // 이 컨트롤러에서 사망 처리를 이미 시작했는지 확인
    private bool deathHandled;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        monsterCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        enemyBase = GetComponent<EnemyBase>();

        if (monsterAnimation == null)
        {
            monsterAnimation =
                GetComponent<MonsterAnimation>();
        }
    }

    private void OnEnable()
    {
        // 풀에서 다시 나온 몬스터의 상태 초기화
        attackTimer = 0f;
        deathHandled = false;

        // 이전에 쓰러졌던 회전값 복구
        monsterAnimation?.ResetFall();

        // 이전 사망 때 꺼졌던 충돌체 복구
        if (monsterCollider != null)
        {
            monsterCollider.enabled = true;
        }

        if (enemyBase != null)
        {
            enemyBase.OnDeathRequested += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (enemyBase != null)
        {
            enemyBase.OnDeathRequested -= HandleDeath;
        }

        StopAllCoroutines();
    }

    private void Update()
    {
        if (enemyBase == null)
        {
            return;
        }

        if (enemyBase.IsKnockback)
        {
            moveDirection = Vector2.zero;
            return;
        }

        if (enemyBase.isDead)
        {
            moveDirection = Vector2.zero;
            return;
        }

        attackTimer += Time.deltaTime;

        CheckDistance();
    }

    private void FixedUpdate()
    {
        if (rb == null || enemyBase == null)
        {
            return;
        }

        if (enemyBase.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (enemyBase.IsKnockback)
        {
            return;
        }

        if (!isTrace)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity =
            moveDirection * enemyBase.GetMoveSpeed();
    }

    private void CheckDistance()
    {
        if (player == null || enemyBase == null)
        {
            moveDirection = Vector2.zero;
            return;
        }

        float distance =
            Vector2.Distance(rb.position, player.position);

        isTrace =
            distance > enemyBase.GetAttackDistance();

        if (isTrace)
        {
            Trace();
        }
        else
        {
            moveDirection = Vector2.zero;
            TryRangedAttack();
        }
    }

    private void Trace()
    {
        if (enemyBase != null &&
        enemyBase.IsKnockback)
        {
            return;
        }

        Move();
    }

    private void Move()
    {
        if (player == null)
        {
            moveDirection = Vector2.zero;
            return;
        }

        moveDirection =
            ((Vector2)player.position - rb.position).normalized;
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
            return;
        }

        Vector2 direction =
            player.position - transform.position;
        direction.Normalize();

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

    /// <summary>
    /// EnemyBase의 체력이 0이 되었을 때 실행되는 사망 처리 함수.
    /// </summary>
    private void HandleDeath(EnemyBase deadEnemy)
    {
        // OnDeathRequested가 여러 번 들어와도 한 번만 처리한다.
        if (deathHandled)
            return;

        deathHandled = true;

        // 실행 중인 공격 관련 코루틴 중지
        StopAllCoroutines();

        // 추적 상태 해제
        isTrace = false;

        moveDirection = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 사망 중 추가 충돌과 피격을 막는다.
        if (monsterCollider != null)
        {
            monsterCollider.enabled = false;
        }

        // SpriteRenderer가 있는 그림 오브젝트를 회전시킨다.
        monsterAnimation?.PlayFall();

        // 사망 효과음 재생
        SoundManager.Instance?.PlaySfx(SFXType.EnemyDie);

        // 쓰러진 모습을 잠시 보여준 뒤 최종 사망 처리
        StartCoroutine(CompleteDeathRoutine());
    }

    private IEnumerator CompleteDeathRoutine()
    {
        // 쓰러지는 연출과 누워 있는 모습을 잠시 보여준다.
        yield return new WaitForSeconds(0.8f);

        if (enemyBase != null)
        {
            // 경험치 생성, HP바 반환, 몬스터 풀 반환
            enemyBase.CompleteDeath();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}