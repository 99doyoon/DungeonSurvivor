using UnityEngine;

public class BossAnimation : ChracterAnimation
{
    [Header("참조")]
    [SerializeField] private BossMonster bossMonster;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform player;

    // Animator 파라미터를 문자열로 반복해서 찾지 않도록 Hash로 저장
    private readonly int isHitHash =
        Animator.StringToHash("isHit");

    private readonly int attackHash =
        Animator.StringToHash("Attack");

    private readonly int patternHash =
        Animator.StringToHash("Pattern");

    private readonly int dieHash =
        Animator.StringToHash("Die");

    private Color originColor;

    private void Awake()
    {
        // 부모 클래스에서 Animator, SpriteRenderer 등을 설정
        SettingAnimation();

        if (sr != null)
        {
            originColor = sr.color;
        }

        // Inspector에 연결하지 않은 경우 자동으로 찾음
        if (bossMonster == null)
        {
            bossMonster = GetComponent<BossMonster>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        CheckFlip();
        CheckMove();
    }

    /// <summary>
    /// 플레이어 위치에 따라 보스 이미지 방향을 전환한다.
    /// </summary>
    public override void CheckFlip()
    {
        if (sr == null || player == null)
            return;

        // 플레이어가 보스의 왼쪽에 있으면 flipX 활성화
        sr.flipX =
            transform.position.x > player.position.x;
    }

    /// <summary>
    /// Rigidbody2D의 이동 속도를 기준으로
    /// 이동 애니메이션을 실행한다.
    /// </summary>
    public override void CheckMove()
    {
        if (anim == null || rb == null)
            return;

        // 아주 작은 흔들림 때문에 Move가 실행되는 것을 방지
        bool isMoving =
            rb.linearVelocity.sqrMagnitude > 0.01f;

        anim.SetBool(isMoveHash, isMoving);
    }

    /// <summary>
    /// 일반 공격 애니메이션을 실행한다.
    /// </summary>
    public void PlayAttack()
    {
        if (anim == null)
            return;

        anim.SetTrigger(attackHash);
    }

    /// <summary>
    /// 특수 패턴 애니메이션을 실행한다.
    /// </summary>
    public void PlayPattern()
    {
        if (anim == null)
            return;

        anim.SetTrigger(patternHash);
    }

    /// <summary>
    /// 피격 애니메이션을 실행한다.
    /// </summary>
    public void PlayHit()
    {
        if (anim == null)
            return;

        anim.SetTrigger(isHitHash);
    }

    /// <summary>
    /// 사망 애니메이션을 실행한다.
    /// </summary>
    public void PlayDie()
    {
        if (anim == null)
            return;

        anim.SetTrigger(dieHash);
    }

    /// <summary>
    /// 죽음 애니메이션의 마지막 프레임에서
    /// Animation Event로 호출한다.
    /// </summary>
    public void OnDieAnimationEnd()
    {
        if (bossMonster == null)
            return;

        bossMonster.CompleteBossDeath();
    }
}