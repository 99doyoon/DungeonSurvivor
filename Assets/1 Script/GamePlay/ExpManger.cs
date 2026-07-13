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
            playerStatus.AddExp(-expForLevel.nextExpRequired[playerStatus.Level]);
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
        if(playerStatus.CurrentExp >= expForLevel.nextExpRequired[playerStatus.Level])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //레벨프린트
    void SetLevelText()
    {
        levelPrint.text = "LEVEL:" + playerStatus.Level;
    }

    //경험치 게이지 설정
    void SetExpGage()
    {
        expSlider.value = ((float)playerStatus.CurrentExp / (float)expForLevel.nextExpRequired[playerStatus.Level]);
    }

    //게임시작시 경험치및 레벨 초기화 만약 세이브로드로 불러오는 기능추가시 수정할것
    void SetGameStartPlayerLevelAndExp()
    {
        //게임 진행상황을 저장하고 실행할경우 함수를 수정할것
        playerStatus.SetExp(0);
        playerStatus.SetLevel(1);
    }
}
