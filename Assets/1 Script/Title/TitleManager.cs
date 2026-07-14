using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("GameScenes");
    }

    public void OpenOption()
    {
        optionPanel.SetActive(true);
    }

    public void CloseOption()
    {
        optionPanel.SetActive(false);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}