using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Result Text")]
    [SerializeField] private TMP_Text killText;
    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text playerLevelText;
    [SerializeField] private TMP_Text skillResultText;

    private void Awake()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(int playerLevel)
    {
        GameResultManager.Instance?.StopRecord();

        gameOverPanel.SetActive(true);

        SetKillText();
        SetSurvivalTimeText();
        SetPlayerLevelText(playerLevel);
        SetSkillResultText();

        Time.timeScale = 0f;
    }

    private void SetKillText()
    {
        int kills = GameResultManager.Instance != null
            ? GameResultManager.Instance.KillCount
            : 0;

        killText.text = $"kills : {kills}";
    }

    private void SetSurvivalTimeText()
    {
        float totalTime = GameResultManager.Instance != null
            ? GameResultManager.Instance.SurvivalTime
            : 0f;

        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime % 60f);

        survivalTimeText.text =
            $"Survival time : {minutes:00}:{seconds:00}";
    }

    private void SetPlayerLevelText(int playerLevel)
    {
        playerLevelText.text = $"Level : Lv.{playerLevel}";
    }

    private void SetSkillResultText()
    {
        if (GameResultManager.Instance == null ||
            GameResultManager.Instance.SkillLevels.Count == 0)
        {
            skillResultText.text = "No skills acquired.";
            return;
        }

        StringBuilder builder = new StringBuilder();

        builder.AppendLine("Acquired Skill");

        foreach (var skill in GameResultManager.Instance.SkillLevels)
        {
            builder.AppendLine($"• {skill.Key}  Lv.{skill.Value}");
        }

        skillResultText.text = builder.ToString();
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void GoToTitle()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("GameStartScenes");
    }
}