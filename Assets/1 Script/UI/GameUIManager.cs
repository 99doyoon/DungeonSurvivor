using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject clearPanel;

    [Header("Clear Result")]
    [SerializeField] private TMP_Text clearKillText;
    [SerializeField] private TMP_Text clearTimeText;

    public GameState CurrentState { get; private set; }
        = GameState.Playing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Time.timeScale = 1f;

        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        clearPanel.SetActive(false);
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (CurrentState == GameState.Paused)
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.Paused;

#if UNITY_EDITOR
        Debug.Log($"PausePanel 연결 상태: {pausePanel}");
#endif
        pausePanel.SetActive(true);
        pausePanel.transform.SetAsLastSibling();

        Debug.Log(
            $"PausePanel activeSelf: {pausePanel.activeSelf}, " +
            $"activeInHierarchy: {pausePanel.activeInHierarchy}"
        );

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        pausePanel.SetActive(false);

        Time.timeScale = 1f;

        CurrentState = GameState.Playing;
    }

    public void ShowGameOver()
    {
        if (CurrentState == GameState.GameOver ||
            CurrentState == GameState.Clear)
        {
            return;
        }

        CurrentState = GameState.GameOver;

        GameResultManager.Instance?.StopRecord();

        pausePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ShowClear()
    {
        if (CurrentState == GameState.Clear ||
            CurrentState == GameState.GameOver)
        {
            return;
        }

        CurrentState = GameState.Clear;

        GameResultManager.Instance?.StopRecord();

        pausePanel.SetActive(false);
        clearPanel.SetActive(true);

        SetClearResult();

        Time.timeScale = 0f;
    }

    private void SetClearResult()
    {
        if (GameResultManager.Instance == null)
        {
            clearKillText.text = "Kill : 0";
            clearTimeText.text = "Clear Time : 00:00";
            return;
        }

        int killCount =
            GameResultManager.Instance.KillCount;

        float totalTime =
            GameResultManager.Instance.SurvivalTime;

        int minutes = Mathf.FloorToInt(totalTime / 60f);
        int seconds = Mathf.FloorToInt(totalTime % 60f);

        clearKillText.text =
            $"Kill : {killCount}";

        clearTimeText.text =
            $"Clear Time : {minutes:00}:{seconds:00}";
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