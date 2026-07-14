using System.Collections;
using UnityEngine;

public enum BossStatus
{
    Idle,      // 대기 상태
    Chase,     // 플레이어 추적 상태
    Attack,    // 일반 공격 상태
    Pattern,   // 특수 패턴 실행 상태
    Damage,    // 피격 상태
    Die        // 사망 상태
}

public class BossMonster : AttackTouch
{
    [Header("보스 데이터")]
    [SerializeField] private BossPatternData patternData;

    [Header("현재 상태")]
    [SerializeField] private BossStatus bossStatus;

    [Header("애니메이션")]
    [SerializeField] private BossAnimation bossAnimation;

    [Header("참조")]
    [SerializeField] private Transform player;

    private EnemyBase enemyBase;
    private Collider2D bossCollider;

    private Rigidbody2D rb;

    // 이전 패턴이 끝난 뒤 흐른 시간
    private float patternTimer;

    // 현재 특수 패턴 코루틴이 실행 중인지 확인
    private bool isPatternRunning;

    // 직전에 실행한 패턴 번호
    // 같은 패턴이 연속으로 실행되는 것을 방지하기 위해 사용
    private int previousPattern = -1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyBase = GetComponent<EnemyBase>();
        bossCollider = GetComponent<Collider2D>();

        if (enemyBase == null)
        {
            Debug.LogError(
                $"{gameObject.name}에 EnemyBase가 없습니다.",
                gameObject
            );
        }

        if (rb == null)
        {
            Debug.LogError(
                $"{gameObject.name}에 Rigidbody2D가 없습니다.",
                gameObject
            );

            enabled = false;
            return;
        }

        if (patternData == null)
        {
            Debug.LogError(
                $"{gameObject.name}에 BossPatternData가 연결되지 않았습니다.",
                gameObject
            );

            enabled = false;
        }

        if (bossAnimation == null)
        {
            bossAnimation =
                GetComponent<BossAnimation>();
        }

        FindPlayer();
    }

    private void OnEnable()
    {
        bossStatus = BossStatus.Idle;
        patternTimer = 0f;
        isPatternRunning = false;
        previousPattern = -1;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (bossCollider != null)
        {
            bossCollider.enabled = true;
        }

        if (enemyBase != null)
        {
            enemyBase.OnDeathRequested += HandleDeath;
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Player 태그를 가진 오브젝트를 찾을 수 없습니다."
            );

            player = null;
            return;
        }

        player = playerObject.transform;
    }

    private void OnDisable()
    {
        if (enemyBase != null)
        {
            enemyBase.OnDeathRequested -= HandleDeath;
        }

        StopAllCoroutines();

        isPatternRunning = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Update()
    {
        // 사망 상태라면 패턴 타이머를 계산하지 않는다.
        if (bossStatus != BossStatus.Die)
        {
            UpdatePatternTimer();
        }

        // 현재 상태에 해당하는 동작 실행
        UpdateBossState();
    }

    /// <summary>
    /// 특수 패턴을 다시 사용할 수 있도록 시간을 계산한다.
    /// 패턴 실행 중에는 타이머가 증가하지 않는다.
    /// </summary>
    private void UpdatePatternTimer()
    {
        if (isPatternRunning)
            return;

        patternTimer += Time.deltaTime;
    }

    /// <summary>
    /// 현재 보스 상태에 맞는 동작을 실행한다.
    /// </summary>
    private void UpdateBossState()
    {
        switch (bossStatus)
        {
            case BossStatus.Idle:
                IdleUpdate();
                break;

            case BossStatus.Chase:
                ChaseUpdate();
                break;

            case BossStatus.Attack:
                AttackUpdate();
                break;

            case BossStatus.Pattern:
                // 패턴 상태의 실제 동작은 코루틴이 담당한다.
                break;

            case BossStatus.Damage:
                DamageUpdate();
                break;

            case BossStatus.Die:
                DieUpdate();
                break;
        }
    }

    /// <summary>
    /// 대기 상태.
    /// 패턴을 사용할 수 있으면 특수 패턴을 실행하고,
    /// 그렇지 않으면 플레이어 추적 상태로 변경한다.
    /// </summary>
    private void IdleUpdate()
    {
        rb.linearVelocity = Vector2.zero;

        if (CanUsePattern())
        {
            SelectPattern();
            return;
        }

        bossStatus = BossStatus.Chase;
    }

    /// <summary>
    /// 플레이어를 추적한다.
    /// 추적 도중 패턴 쿨타임이 끝나면 패턴을 실행한다.
    /// </summary>
    private void ChaseUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (CanUsePattern())
        {
            SelectPattern();
            return;
        }

        // 보스에서 플레이어를 향하는 방향
        Vector2 direction =
            ((Vector2)player.position - rb.position).normalized;

        rb.linearVelocity =
            direction * patternData.moveSpeed;
    }

    /// <summary>
    /// 일반 공격 상태.
    /// 아직 일반 공격을 구현하지 않았기 때문에 바로 Idle로 돌아간다.
    /// </summary>
    private void AttackUpdate()
    {
        rb.linearVelocity = Vector2.zero;

        bossStatus = BossStatus.Idle;
    }

    /// <summary>
    /// 피격 상태.
    /// 현재는 이동을 멈춘 뒤 바로 Idle 상태로 돌아간다.
    /// </summary>
    private void DamageUpdate()
    {
        rb.linearVelocity = Vector2.zero;

        bossStatus = BossStatus.Idle;
    }

    /// <summary>
    /// 사망 상태에서는 이동만 정지한다.
    /// </summary>
    private void DieUpdate()
    {
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// 보스를 사망 상태로 변경한다.
    /// 체력이 0 이하가 되는 순간 한 번 호출해야 한다.
    /// </summary>
    public void Die()
    {
        if (bossStatus == BossStatus.Die)
            return;

        bossStatus = BossStatus.Die;

        StopAllCoroutines();

        isPatternRunning = false;
        rb.linearVelocity = Vector2.zero;

    }

    /// <summary>
    /// 현재 보스가 특수 패턴을 사용할 수 있는지 검사한다.
    /// </summary>
    private bool CanUsePattern()
    {
        // 패턴 데이터가 없으면 실행 불가
        if (patternData == null)
            return false;

        // 이미 패턴을 실행 중이면 실행 불가
        if (isPatternRunning)
            return false;

        // 패턴 쿨타임이 끝나지 않았으면 실행 불가
        if (patternTimer < patternData.patternCooldown)
            return false;

        // 특정 상태에서는 새로운 패턴 실행 불가
        if (bossStatus == BossStatus.Pattern ||
            bossStatus == BossStatus.Attack ||
            bossStatus == BossStatus.Damage ||
            bossStatus == BossStatus.Die)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 사용할 특수 패턴을 선택하고 실행한다.
    /// 패턴 실행의 시작 지점이다.
    /// </summary>
    private void SelectPattern()
    {
        if (!CanUsePattern())
            return;

        // 패턴을 사용하는 순간 쿨타임 초기화
        patternTimer = 0f;

        // 일반 행동을 멈추기 위해 Pattern 상태로 변경
        bossStatus = BossStatus.Pattern;

        const int patternCount = 3;

        int selectedPattern;

        // 이전 패턴과 다른 패턴을 선택한다.
        do
        {
            selectedPattern = Random.Range(0, patternCount);
        }
        while (selectedPattern == previousPattern);

        previousPattern = selectedPattern;

        switch (selectedPattern)
        {
            // 원형 투사체 패턴
            case 0:
                StartCoroutine(CircleShotPattern());
                break;

            // 부채꼴 투사체 패턴
            case 1:
                StartCoroutine(FanShotPattern());
                break;

            // 돌진 패턴
            case 2:
                StartCoroutine(DashPattern());
                break;

            default:
                FinishPattern();
                break;
        }
    }

    /// <summary>
    /// 보스 주변 360도 방향으로 투사체를 발사한다.
    /// </summary>
    private void CircleShot()
    {
        int bulletCount = patternData.circleBulletCount;

        if (bulletCount <= 0)
            return;

        // 투사체 사이의 각도
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = angleStep * i;

            MonsterBulletAttackTouch bullet =
                ObjectPool.instance.GetObject<MonsterBulletAttackTouch>(
                    patternData.bulletPoolType
                );

            if (bullet == null)
                continue;

            // 보스 위치에서 투사체 발사
            bullet.transform.position = transform.position;

            // 투사체의 transform.right가 발사 방향이 되도록 설정
            bullet.transform.rotation =
                Quaternion.Euler(0f, 0f, angle);

            bullet.Init(
                patternData.bulletDamage,
                patternData.circleBulletSpeed
            );
        }
    }

    /// <summary>
    /// 플레이어 방향을 중심으로 부채꼴 형태의 투사체를 발사한다.
    /// </summary>
    private void FanShot()
    {
        if (player == null)
            return;

        int bulletCount = patternData.fanBulletCount;

        if (bulletCount <= 0)
            return;

        // 보스에서 플레이어를 향하는 방향
        Vector2 direction =
            (Vector2)player.position - rb.position;

        // 방향을 각도로 변환
        float centerAngle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        for (int i = 0; i < bulletCount; i++)
        {
            // 탄환들을 가운데 기준으로 좌우에 배치한다.
            float offset =
                (i - (bulletCount - 1) / 2f)
                * patternData.fanSpreadAngle;

            float angle = centerAngle + offset;

            MonsterBulletAttackTouch bullet =
                ObjectPool.instance.GetObject<MonsterBulletAttackTouch>(
                    patternData.bulletPoolType
                );

            if (bullet == null)
                continue;

            bullet.transform.position = transform.position;

            bullet.transform.rotation =
                Quaternion.Euler(0f, 0f, angle);

            bullet.Init(
                patternData.bulletDamage,
                patternData.fanBulletSpeed
            );
        }
    }

    /// <summary>
    /// 원형으로 투사체를 발사하는 패턴.
    /// </summary>
    private IEnumerator CircleShotPattern()
    {
        isPatternRunning = true;

        // 패턴 중에는 보스 이동 정지
        rb.linearVelocity = Vector2.zero;

        // 특수 패턴 애니메이션 실행
        bossAnimation?.PlayHit();

        // 공격 예고 시간
        yield return new WaitForSeconds(
            patternData.patternWarningTime
        );

        // 예고 도중 죽었으면 발사하지 않는다.
        if (bossStatus == BossStatus.Die)
            yield break;

        // 실제 투사체 발사
        CircleShot();

        // 패턴 사용 후 후딜레이
        yield return new WaitForSeconds(
            patternData.patternAfterDelay
        );

        FinishPattern();
    }

    /// <summary>
    /// 플레이어 방향으로 부채꼴 투사체를 발사하는 패턴.
    /// </summary>
    private IEnumerator FanShotPattern()
    {
        isPatternRunning = true;

        rb.linearVelocity = Vector2.zero;

        bossAnimation?.PlayAttack();

        // 공격 예고 시간
        yield return new WaitForSeconds(
            patternData.patternWarningTime
        );

        if (bossStatus == BossStatus.Die)
            yield break;

        // 실제 부채꼴 투사체 발사
        FanShot();

        // 패턴 사용 후 후딜레이
        yield return new WaitForSeconds(
            patternData.patternAfterDelay
        );

        FinishPattern();
    }

    /// <summary>
    /// 패턴을 시작할 때 바라본 플레이어 방향으로 돌진한다.
    /// </summary>
    private IEnumerator DashPattern()
    {
        isPatternRunning = true;

        rb.linearVelocity = Vector2.zero;

        bossAnimation?.PlayPattern();

        if (player == null)
        {
            FinishPattern();
            yield break;
        }

        // 예고 시간 중 플레이어가 이동해도
        // 처음 저장한 방향으로 돌진하도록 방향을 미리 저장한다.
        Vector2 dashDirection =
            ((Vector2)player.position - rb.position).normalized;

        // 돌진 예고 시간
        yield return new WaitForSeconds(
            patternData.patternWarningTime
        );

        if (bossStatus == BossStatus.Die)
            yield break;

        // 저장해둔 방향으로 돌진
        rb.linearVelocity =
            dashDirection * patternData.dashSpeed;

        // 돌진 지속 시간
        yield return new WaitForSeconds(
            patternData.dashDuration
        );

        rb.linearVelocity = Vector2.zero;

        // 돌진 후 후딜레이
        yield return new WaitForSeconds(
            patternData.patternAfterDelay
        );

        FinishPattern();
    }

    [SerializeField] private float dieAnimationTime = 2f;

    private void HandleDeath(EnemyBase deadEnemy)
    {
        if (bossStatus == BossStatus.Die)
            return;

        bossStatus = BossStatus.Die;

        // 실행 중인 이동 및 패턴 중지
        StopAllCoroutines();

        isPatternRunning = false;
        rb.linearVelocity = Vector2.zero;

        // 죽은 뒤 충돌하지 않도록 처리
        if (bossCollider != null)
        {
            bossCollider.enabled = false;
        }

        // 죽음 애니메이션 재생
        bossAnimation?.PlayDie();
    }

    /// <summary>
    /// 죽음 애니메이션 마지막 프레임의
    /// Animation Event에서 호출된다.
    /// </summary>
    public void CompleteBossDeath()
    {
        if (bossStatus != BossStatus.Die)
            return;

        if (enemyBase == null)
        {
            Debug.LogError(
                $"{gameObject.name}에 EnemyBase가 없습니다.",
                gameObject
            );

            gameObject.SetActive(false);
            return;
        }

        // 경험치 생성 후 보스를 풀에 반환
        enemyBase.CompleteDeath();
    }

    /// <summary>
    /// 모든 패턴이 끝날 때 공통으로 호출한다.
    /// </summary>
    private void FinishPattern()
    {
        // 사망 상태를 다른 상태로 덮어쓰지 않는다.
        if (bossStatus == BossStatus.Die)
            return;

        rb.linearVelocity = Vector2.zero;

        isPatternRunning = false;

        // 다시 일반 행동으로 돌아간다.
        bossStatus = BossStatus.Idle;
    }
}