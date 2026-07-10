using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpManger : MonoBehaviour
{
    [SerializeField] Slider expSlider;
    [SerializeField] TMP_Text levelPrint;
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] ExpForLevel expForLevel;

    public void GetExp(int getExp)
    {
        playerStatus.AddExp(getExp);
        while(CheckLevelUp())
        {
            playerStatus.AddExp(-expForLevel.nextExpRequired[playerStatus.Level]);
            playerStatus.AddLevel(1);
        }
        SetExpGage();
    }

    bool CheckLevelUp()
    {
        if(playerStatus.CurrentExp > expForLevel.nextExpRequired[playerStatus.Level])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void SetExpGage()
    {
        expSlider.value = ((float)playerStatus.CurrentExp / (float)expForLevel.nextExpRequired[playerStatus.Level]);
    }
}
