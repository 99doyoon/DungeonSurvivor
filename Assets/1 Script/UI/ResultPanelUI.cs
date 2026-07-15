using System.Text;
using TMPro;
using UnityEngine;

public class ResultPanelUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject resultPanel;

    [Header("Texts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text playerLevelText;
    [SerializeField] private TMP_Text skillResultText;

    public void Show(ResultType resultType, int playerLevel)
    {
        resultPanel.SetActive(true);
        resultPanel.transform.SetAsLastSibling();

        SetTitle(resultType);
        SetKillText();
        SetSurvivalTimeText(resultType);
        SetPlayerLevelText(playerLevel);
        SetSkillResultText();
    }

    private void SetTitle(ResultType resultType)
    {
        switch (resultType)
        {
            case ResultType.GameOver:
                titleText.text = "GAME OVER";
                break;

            case ResultType.Clear:
                titleText.text = "STAGE CLEAR";
                break;
        }
    }

    private void SetKillText()
    {
        int killCount = GameResultManager.Instance != null
            ? GameResultManager.Instance.KillCount
            : 0;

        killText.text = $"처치한 몬스터 : {killCount}";
    }

    private void SetSurvivalTimeText(ResultType resultType)
    {
        float totalTime = GameResultManager.Instance != null
            ? GameResultManager.Instance.SurvivalTime
            : 0f;

        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime % 60f);

        string label = resultType == ResultType.Clear
            ? "클리어 시간"
            : "생존 시간";

        survivalTimeText.text =
            $"{label} : {minutes:00}:{seconds:00}";
    }

    private void SetPlayerLevelText(int playerLevel)
    {
        playerLevelText.text = $"최종 레벨 : Lv.{playerLevel}";
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

    public void Hide()
    {
        resultPanel.SetActive(false);
    }
}