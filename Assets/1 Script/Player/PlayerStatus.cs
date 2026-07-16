using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : CharacterStatus, IHit
{
    PlayerAnimationController playerAnimationController;

    [SerializeField] private ResultPanelUI resultPanelUI;

    [Header("사망 확인")]
    public bool isDead { get; private set; }

    [Header("사망 처리")]
    [SerializeField] private float gameOverDelay = 1f;

    // 플레이어 이동을 담당하는 스크립트를 Inspector에서 연결
    [SerializeField] private MonoBehaviour moveController;

    // 자동 공격을 담당하는 스크립트를 Inspector에서 연결
    [SerializeField] private MonoBehaviour attackController;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private Coroutine deathCoroutine;

    [Header("레벨 및 경험치")]
    [field: SerializeField]
    public int Level { get; private set; }

    [field: SerializeField]
    public int CurrentExp { get; private set; }

    [Header("플레이어 능력치")]
    [field: SerializeField]
    public float Damage { get; private set; }

    [field: SerializeField]
    public float AttackDelay { get; private set; }

    [field: SerializeField]
    public int ProjectileCount { get; private set; }

    [field: SerializeField]
    public float ProjectileSpeed { get; private set; }

    [Header("자동 회복")]
    [field: SerializeField]
    public int AutoHealAmount { get; private set; }

    [field: SerializeField]
    public float AutoHealInterval { get; private set; }

    private Coroutine autoHealCoroutine;

    [field: SerializeField]
    public float AttackRange { get; private set; }

    //player hp ui
    [SerializeField] Slider hpSlider;
    [SerializeField] TMP_Text hpPrint;

    void Start()
    {
        //MoveSpeed = 5;
        //base.maxHp = 100;
        //base.nowHp = base.maxHp;
        //Level = 1;
        //CurrentExp = 0;

        //playerAnimationController = GetComponentInChildren<PlayerAnimationController>();

        //isHit = true;
        //takeDamageDelay = 1;
        //wait = new WaitForSeconds(takeDamageDelay);

        StartGameStatusInit();
    }


    //게임 시작시 초기 상태 설정. player데이터도 데이터 설정 방식이 바뀌면 수정할 것 
    void StartGameStatusInit()
    {
        isDead = false;

        MoveSpeed = 5f;

        maxHp = 100;
        nowHp = maxHp;

        Level = 1;
        CurrentExp = 0;

        Damage = 5f;
        AttackDelay = 1f;
        ProjectileCount = 1;
        ProjectileSpeed = 5f;

        AutoHealAmount = 0;
        AutoHealInterval = 5f;

        playerAnimationController =
            GetComponentInChildren<PlayerAnimationController>();

        // 사망 처리에 사용할 컴포넌트
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        isHit = true;
        takeDamageDelay = 1f;
        wait = new WaitForSeconds(takeDamageDelay);

        if (hpSlider == null)
        {
            GameObject hpBarObject = GameObject.Find("HpBar");

            if (hpBarObject != null)
            {
                hpSlider = hpBarObject.GetComponent<Slider>();
            }
        }

        if (hpPrint == null)
        {
            GameObject hpPrintObject = GameObject.Find("HpPrint");

            if (hpPrintObject != null)
            {
                hpPrint =
                    hpPrintObject.GetComponent<TMP_Text>();
            }
        }

        StartAutoHeal();
        UpdateHpUI();
    }

    private void UpdateHpUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = (float)nowHp / maxHp;
        }

        if (hpPrint != null)
        {
            hpPrint.text = $"HP : {nowHp} / {maxHp}";
        }
    }

    public override void TakeDamage(float damage)
    {
        // 이미 죽은 플레이어는 추가 피해를 받지 않는다.
        if (isDead)
            return;

        base.TakeDamage(damage);
        UpdateHpUI();
    }

    /// <summary>
    /// 플레이어의 체력이 0이 되었을 때 호출된다.
    /// 쓰러지는 연출이 끝난 뒤 게임오버 창을 표시한다.
    /// </summary>
    protected override void Die()
    {
        // 여러 공격이 동시에 들어와도 사망 처리를 한 번만 실행
        if (isDead)
            return;

        isDead = true;

#if UNITY_EDITOR
        Debug.Log("Player Die");
#endif

        // 자동 회복 중지
        if (autoHealCoroutine != null)
        {
            StopCoroutine(autoHealCoroutine);
            autoHealCoroutine = null;
        }

        // 플레이어 이동 정지
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // 이동 입력 중지
        if (moveController != null)
        {
            moveController.enabled = false;
        }

        // 자동 공격 중지
        if (attackController != null)
        {
            attackController.enabled = false;
        }

        // 사망 후 추가 충돌과 피격 방지
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        // 더 이상 피격 처리하지 않음
        isHit = false;

        // 플레이어의 SpriteRenderer 오브젝트를 회전시킨다.
        playerAnimationController?.PlayFall();

        // 쓰러진 모습을 보여준 뒤 게임오버 창 표시
        deathCoroutine =
            StartCoroutine(ShowGameOverRoutine());
    }

    /// <summary>
    /// 플레이어가 쓰러지는 모습을 잠시 보여준 뒤
    /// 게임오버 패널을 표시한다.
    /// </summary>
    private IEnumerator ShowGameOverRoutine()
    {
        yield return new WaitForSeconds(gameOverDelay);

        if (GameUIManager.Instance == null)
        {
            Debug.LogError(
                "GameUIManager.Instance가 null입니다.",
                gameObject
            );

            yield break;
        }

        GameUIManager.Instance.ShowGameOver();

        deathCoroutine = null;
    }

    public bool Hit()
    {
        // 사망 상태에서는 피격 효과를 실행하지 않는다.
        if (isDead)
            return false;

        if (!isHit)
            return false;

        SoundManager.Instance?.PlaySfx(
            SFXType.PlayerHit
        );

        StartCoroutine(SetIsHit());

        playerAnimationController?.HitAnimation();

        return true;
    }

    //takeDamageDelay동안 데미지를 입지않기위해 isHit을 false로 만든다
    public IEnumerator SetIsHit()
    {
        isHit = false;
        yield return wait;
        isHit = true;
    }

    public void AddExp(int exp)
    {
        CurrentExp += exp;
    }

    public void AddLevel(int level)
    {
        Level += level;
    }

    public void SetExp(int exp)
    {
        CurrentExp = exp;
    }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void IncreaseDamage(float value)
    {
        Damage += value;
    }

    public void IncreaseAttackSpeed(float value)
    {
        AttackDelay -= value;
        AttackDelay = Mathf.Max(0.1f, AttackDelay);
    }

    public void IncreaseMoveSpeed(float value)
    {
        MoveSpeed += value;
    }

    public void IncreaseMaxHp(int value)
    {
        maxHp += value;
        nowHp += value;

        UpdateHpUI();
    }


    public void AddProjectile(int value)
    {
        ProjectileCount += value;
    }

    public void IncreaseProjectileSpeed(float value)
    {
        ProjectileSpeed += value;
    }
    private void StartAutoHeal()
    {
        if (autoHealCoroutine != null)
        {
            StopCoroutine(autoHealCoroutine);
        }

        autoHealCoroutine = StartCoroutine(AutoHealRoutine());
    }

    private IEnumerator AutoHealRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(
                AutoHealInterval
            );

            if (isDead)
                yield break;

            if (AutoHealAmount <= 0)
                continue;

            if (nowHp >= maxHp)
                continue;

            Heal(AutoHealAmount);
        }

        autoHealCoroutine = null;
    }

    public void Heal(int value)
    {
        if (isDead)
            return;

        nowHp = Mathf.Min(
            nowHp + value,
            maxHp
        );

        UpdateHpUI();
    }

    public void IncreaseAutoHealAmount(int value)
    {
        AutoHealAmount += value;
    }

    public void DecreaseAutoHealInterval(float value)
    {
        AutoHealInterval -= value;
        AutoHealInterval = Mathf.Max(0.5f, AutoHealInterval);

        // WaitForSeconds가 기존 시간으로 만들어져 있을 수 있으므로 재시작
        StartAutoHeal();
    }
}
