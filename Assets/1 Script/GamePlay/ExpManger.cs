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
        //2단레벨업을 할경우
        while(CheckLevelUp())
        {
            playerStatus.AddExp(-expForLevel.nextExpRequired[playerStatus.Level]);
            playerStatus.AddLevel(1);
            SetLevelText();
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

    void SetLevelText()
    {
        levelPrint.text = "LEVEL:" + playerStatus.Level;
    }

    void SetExpGage()
    {
        expSlider.value = ((float)playerStatus.CurrentExp / (float)expForLevel.nextExpRequired[playerStatus.Level]);
    }

    void SetGameStartPlayerLevelAndExp()
    {
        //게임 진행상황을 저장하고 실행할경우 함수를 수정할것
        playerStatus.SetExp(0);
        playerStatus.SetLevel(1);
    }
}
