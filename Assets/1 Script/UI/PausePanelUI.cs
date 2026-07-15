using System.Text;
using TMPro;
using UnityEngine;

public class PausePanelUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    [Header("Current Status")]
    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text playerLevelText;
    [SerializeField] private TMP_Text skillResultText;

    public void Show(int playerLevel)
    {
        pausePanel.SetActive(true);
        pausePanel.transform.SetAsLastSibling();

        SetCurrentStatus(playerLevel);
        SetSkillResultText();
    }

    public void Hide()
    {
        pausePanel.SetActive(false);
    }

    private void SetCurrentStatus(int playerLevel)
    {
        if (GameResultManager.Instance == null)
        {
            killText.text = "처치 수 : 0";
            survivalTimeText.text = "생존 시간 : 00:00";
            playerLevelText.text = $"현재 레벨 : Lv.{playerLevel}";
            return;
        }

        int killCount =
            GameResultManager.Instance.KillCount;

        float totalTime =
            GameResultManager.Instance.SurvivalTime;

        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime % 60f);

        killText.text = $"처치 수 : {killCount}";

        survivalTimeText.text =
            $"생존 시간 : {minutes:00}:{seconds:00}";

        playerLevelText.text =
            $"현재 레벨 : Lv.{playerLevel}";
    }

    private void SetSkillResultText()
    {
        if (GameResultManager.Instance == null ||
            GameResultManager.Instance.SkillLevels.Count == 0)
        {
            skillResultText.text = "획득한 스킬이 없습니다.";
            return;
        }

        StringBuilder builder = new StringBuilder();

        foreach (var skill in GameResultManager.Instance.SkillLevels)
        {
            builder.AppendLine(
                $"• {skill.Key}  Lv.{skill.Value}"
            );
        }

        skillResultText.text = builder.ToString();
    }
}