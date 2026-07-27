using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ExpManger : MonoBehaviour
{
    [SerializeField] Slider expSlider;
    [SerializeField] TMP_Text levelPrint;
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] ExpForLevel expForLevel;

    [SerializeField] private RewardManager rewardManager;

    // 레벨업 보상을 선택 중인지 확인한다.
    private bool isSelectingReward;

    /// <summary>
    /// 레벨 관련 상태와 ui와 연결
    /// </summary>
    private void Awake()
    {

        if (expSlider == null)
        {
            expSlider = GameObject.Find("ExpBar").GetComponent<Slider>();
        }
        else
        {

        }

        if(levelPrint == null)
        {
            levelPrint = GameObject.Find("LevelPrint").GetComponent<TMP_Text>();
        }
        else
        {

        }
    }

    private void Start()
    {
        SetGameStartPlayerLevelAndExp();
        SetLevelText();
        SetExpGage();
    }

    private int GetLevelIndex()
    {
        return playerStatus.Level - 1;
    }

    /// <summary>
    /// 경험치를 획득하고, 조건을 만족하면 한 번만 레벨업한다.
    /// 초과 경험치는 그대로 남겨둔다.
    /// </summary>
    public void GetExp(int getExp)
    {
        if (getExp <= 0)
            return;

        playerStatus.AddExp(getExp);

        // 이미 보상창이 열려 있다면 경험치부터 누적하고
        // 다음 레벨업은 보상 선택 후 검사한다.
        if (isSelectingReward)
        {
            SetExpGage();
            return;
        }

        TryLevelUpOnce();
        SetExpGage();
    }

    /// <summary>
    /// 레벨업이 가능할 경우 한 번만 처리하고 보상창을 연다.
    /// </summary>
    private void TryLevelUpOnce()
    {
        int levelIndex = GetLevelIndex();

        if (levelIndex < 0 ||
            levelIndex >= expForLevel.nextExpRequired.Count)
        {
            return;
        }

        int requiredExp =
            expForLevel.nextExpRequired[levelIndex];


        if (!CheckLevelUp())
            return;

        // 필요한 경험치 차감
        playerStatus.AddExp(-requiredExp);

        // 레벨 1 증가
        playerStatus.AddLevel(1);


        SetLevelText();
        SetExpGage();

        SoundManager.Instance?.PlaySfx(SFXType.LevelUp);

        isSelectingReward = true;

        rewardManager?.OpenReward();
    }

    /// <summary>
    /// 현재 경험치가 레벨업 조건을 만족하는지 검사한다.
    /// </summary>
    private bool CheckLevelUp()
    {
        int levelIndex = GetLevelIndex();

        if (levelIndex < 0 ||
            levelIndex >= expForLevel.nextExpRequired.Count)
        {
            return false;
        }

        int requiredExp =
            expForLevel.nextExpRequired[levelIndex];

        return playerStatus.CurrentExp >= requiredExp;
    }

    //레벨프린트
    void SetLevelText()
    {
        levelPrint.text = "LEVEL:" + playerStatus.Level;
    }

    //경험치 게이지 설정
    private void SetExpGage()
    {
        int levelIndex = GetLevelIndex();

        bool isMaxLevel =
            levelIndex < 0 ||
            levelIndex >= expForLevel.nextExpRequired.Count;

        if (isMaxLevel)
        {
            expSlider.value = 1f;
            levelPrint.text = $"Lv.{playerStatus.Level} MAX";
            return;
        }

        int requiredExp =
            expForLevel.nextExpRequired[levelIndex];

        expSlider.value =
            (float)playerStatus.CurrentExp / requiredExp;

        levelPrint.text = $"Lv.{playerStatus.Level}";
    }

    //게임시작시 경험치및 레벨 초기화 만약 세이브로드로 불러오는 기능추가시 수정할것
    void SetGameStartPlayerLevelAndExp()
    {
        //게임 진행상황을 저장하고 실행할경우 함수를 수정할것
        playerStatus.SetExp(0);
        playerStatus.SetLevel(1);
    }

    /// <summary>
    /// 레벨업 보상 선택이 끝났을 때 RewardManager에서 호출한다.
    /// </summary>
    public void CompleteRewardSelection()
    {
        isSelectingReward = false;

        // 남은 경험치가 다음 레벨 조건을 만족한다면
        // 다음 보상을 한 번 더 표시한다.
        TryLevelUpOnce();

        SetExpGage();
    }
}
