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
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject optionPanel;

    [Header("UI Controllers")]
    [SerializeField] private ResultPanelUI resultPanelUI;
    [SerializeField] private PausePanelUI pausePanelUI;

    [Header("Player")]
    [SerializeField] private PlayerStatus playerStatus;

    /// <summary>
    /// 현재 게임의 진행 상태.
    /// Playing, Paused, GameOver, Clear 등의 상태를 저장한다.
    /// </summary>
    public GameState CurrentState { get; private set; }
        = GameState.Playing;

    private void Awake()
    {
        // GameUIManager 싱글톤 생성
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        /*
         * 이전 게임오버나 일시정지로 인해
         * Time.timeScale이 0인 상태가 남아 있을 수 있으므로
         * 게임 씬이 시작될 때 정상 속도로 복구한다.
         *
         * 게임 시작 카운트다운을 사용한다면
         * GameStartCountdown에서 다시 0으로 설정한다.
         */
        Time.timeScale = 1f;

        // 게임 시작 시 모든 보조 패널을 닫아둔다.
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }
    }

    /// <summary>
    /// ESC 등의 입력으로 일시정지 상태를 전환한다.
    /// 옵션 창이 열려 있다면 옵션 창을 먼저 닫는다.
    /// </summary>
    public void TogglePause()
    {
        if (optionPanel != null &&
            optionPanel.activeSelf)
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

    /// <summary>
    /// 게임을 일시정지하고 Pause 패널을 표시한다.
    /// </summary>
    public void PauseGame()
    {
        // 플레이 중일 때만 일시정지 가능
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.Paused;

        // 현재 플레이어 레벨을 Pause 패널에 표시
        if (pausePanelUI != null)
        {
            pausePanelUI.Show(playerStatus.Level);
        }

        // 게임 시간 정지
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 일시정지를 해제하고 게임을 다시 진행한다.
    /// </summary>
    public void ResumeGame()
    {
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        if (pausePanelUI != null)
        {
            pausePanelUI.Hide();
        }

        Time.timeScale = 1f;
        CurrentState = GameState.Playing;
    }

    /// <summary>
    /// 플레이어가 사망했을 때 게임오버 결과창을 표시한다.
    /// </summary>
    public void ShowGameOver()
    {
        // 이미 일시정지, 클리어, 게임오버 상태라면 중복 실행 방지
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.GameOver;

        // 플레이 시간이나 결과 기록을 중지한다.
        GameResultManager.Instance?.StopRecord();

        // 일시정지 패널이 열려 있다면 닫는다.
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // 게임오버 결과 표시
        if (resultPanelUI != null)
        {
            resultPanelUI.Show(
                ResultType.GameOver,
                playerStatus.Level
            );
        }

        // 결과창 표시 후 게임 정지
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 보스를 처치하거나 게임을 완료했을 때
    /// 클리어 결과창을 표시한다.
    /// </summary>
    public void ShowClear()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        CurrentState = GameState.Clear;

        GameResultManager.Instance?.StopRecord();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (resultPanelUI != null)
        {
            resultPanelUI.Show(
                ResultType.Clear,
                playerStatus.Level
            );
        }

        Time.timeScale = 0f;
    }

    /// <summary>
    /// 현재 게임 씬을 다시 불러와 게임을 재시작한다.
    /// 게임오버로 멈춰 있던 시간을 먼저 복구해야 한다.
    /// </summary>
    public void RetryGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    /// <summary>
    /// 타이틀 씬으로 이동한다.
    /// </summary>
    public void GoToTitle()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("GameStartScenes");
    }

    /// <summary>
    /// 일시정지 상태에서 옵션 창을 연다.
    /// </summary>
    public void OpenOption()
    {
        // 게임 일시정지 중에만 옵션 창을 열 수 있다.
        if (CurrentState != GameState.Paused)
        {
            return;
        }

        if (pausePanelUI != null)
        {
            pausePanelUI.Hide();
        }

        if (optionPanel != null)
        {
            optionPanel.SetActive(true);

            // 다른 UI보다 앞에 보이도록 마지막 자식으로 이동
            optionPanel.transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// 옵션 창을 닫고,
    /// 일시정지 상태라면 Pause 패널을 다시 표시한다.
    /// </summary>
    public void CloseOption()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(false);
        }

        if (CurrentState == GameState.Paused &&
            pausePanelUI != null)
        {
            pausePanelUI.Show(playerStatus.Level);
        }
    }
}