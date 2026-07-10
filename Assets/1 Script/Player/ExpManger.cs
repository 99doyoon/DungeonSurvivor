using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpManger : MonoBehaviour
{
    [SerializeField] Slider expSlider;
    [SerializeField] TMP_Text levelPrint;
    [SerializeField] PlayerStatus playerStatus;
    [SerializeField] ExpForLevel expForLevel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckLevelup()
    {

    }

    void SetExpGage()
    {
        expSlider.value = ((float)playerStatus.CurrentExp / (float)expForLevel.nextExpRequired[playerStatus.Level]);
    }
}
