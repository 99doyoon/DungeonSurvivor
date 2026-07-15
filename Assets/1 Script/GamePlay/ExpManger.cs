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
#if UNITY_EDITOR
            Debug.Log($"expSlider != null");
#endif
        }

        if(levelPrint == null)
        {
            levelPrint = GameObject.Find("LevelPrint").GetComponent<TMP_Text>();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"levelPrint != null");
#endif
        }
    }

    private void Start()
    {
        SetGameStartPlayerLevelAndExp();
        SetLevelText();
        SetExpGage();
    }

    //레벨업 구슬을 먹었을시
    public void GetExp(int getExp)
    {
        playerStatus.AddExp(getExp);
        //2단레벨업을 할경우에도 1번하고 그다음에 또하게
        while(CheckLevelUp())
        {
            int requiredExp =
            expForLevel.nextExpRequired[playerStatus.Level];

            playerStatus.AddExp(-requiredExp);
            playerStatus.AddLevel(1);
            SetLevelText();

            // 레벨업 보상 표시
            rewardManager.OpenReward();
        }
        SetExpGage();
    }

    //레벨업량 체크
    bool CheckLevelUp()
    {
        // 현재 레벨로 경험치 테이블에 접근할 수 없다면 만렙
        if (playerStatus.Level >= expForLevel.nextExpRequired.Count)
        {
            return false;
        }

        int requiredExp =
            expForLevel.nextExpRequired[playerStatus.Level];

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
        bool isMaxLevel =
            playerStatus.Level >= expForLevel.nextExpRequired.Count;

        if (isMaxLevel)
        {
            // 만렙 이후에도 경험치 획득량을 보여주고 싶다면
            // Slider가 계속 가득 찬 상태로 표시
            expSlider.value = 1f;
            levelPrint.text = $"Lv.{playerStatus.Level} MAX";
            return;
        }

        int requiredExp =
            expForLevel.nextExpRequired[playerStatus.Level];

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
}
