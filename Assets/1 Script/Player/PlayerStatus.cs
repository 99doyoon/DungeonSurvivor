using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : CharacterStatus, IHit
{
    PlayerAnimationController playerAnimationController;

    [SerializeField] private ResultPanelUI resultPanelUI;

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

        isHit = true;
        takeDamageDelay = 1f;
        wait = new WaitForSeconds(takeDamageDelay);

        if (hpSlider == null)
        {
            hpSlider = GameObject.Find("HpBar").GetComponent<Slider>();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"hpSlider != null");
#endif
        }

        if (hpPrint == null)
        {
            hpPrint = GameObject.Find("HpPrint").GetComponent<TMP_Text>();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"hpPrint != null");
#endif
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
        base.TakeDamage(damage);
        UpdateHpUI();
    }

    protected override void Die()
    {
#if UNITY_EDITOR
        Debug.Log("Player Die");
#endif
        if (GameUIManager.Instance == null)
        {
            Debug.LogError("GameUIManager.Instance가 null입니다.");
            return;
        }

        GameUIManager.Instance.ShowGameOver();
    }
    public bool Hit()
    {
        if(isHit)
        {
            StartCoroutine(SetIsHit());
            playerAnimationController.HitAnimation();
            return true;
        }
        else
        {
            return false;
        }
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

#if UNITY_EDITOR
        Debug.Log(
         $"[PlayerStatus] 증가량: {value}, 현재 Damage: {Damage}");
#endif
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
        while (true)
        {
            yield return new WaitForSeconds(AutoHealInterval);

            if (AutoHealAmount <= 0)
            {
                continue;
            }

            if (nowHp >= maxHp)
            {
                continue;
            }

            Heal(AutoHealAmount);
        }
    }

    public void Heal(int value)
    {
        nowHp = Mathf.Min(nowHp + value, maxHp);

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
