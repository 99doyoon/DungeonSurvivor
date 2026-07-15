using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ResultType
{
    GameOver,
    Clear
}

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private GameObject optionPanel;

    [SerializeField] private ResultPanelUI resultPanelUI;
    [SerializeField] private PausePanelUI pausePanelUI;
    [SerializeField] private PlayerStatus playerStatus;

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
        if (optionPanel.activeSelf)
        {
            CloseOption();
            return;
        }

        switch (CurrentState)
        {
            case GameState.Playing:
                PauseGame();
                break;

            case GameState.Paused:
                ResumeGame();
                break;
        }
    }

    public void PauseGame()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.Paused;

        pausePanelUI.Show(playerStatus.Level);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        pausePanelUI.Hide();

        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
    }

    public void ShowGameOver()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.GameOver;

        GameResultManager.Instance?.StopRecord();

        pausePanel.SetActive(false);

        resultPanelUI.Show(
            ResultType.GameOver,
            playerStatus.Level
        );

        Time.timeScale = 0f;
    }

    public void ShowClear()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.Clear;

        GameResultManager.Instance?.StopRecord();

        pausePanel.SetActive(false);

        resultPanelUI.Show(
            ResultType.Clear,
            playerStatus.Level
        );

        Time.timeScale = 0f;
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

    public void OpenOption()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        pausePanelUI.Hide();

        optionPanel.SetActive(true);
        optionPanel.transform.SetAsLastSibling();
    }

    public void CloseOption()
    {
        optionPanel.SetActive(false);

        if (CurrentState == GameState.Paused)
        {
            pausePanelUI.Show(playerStatus.Level);
        }
    }
}