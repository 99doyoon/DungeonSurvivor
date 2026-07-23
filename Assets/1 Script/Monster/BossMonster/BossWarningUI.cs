using TMPro;
using UnityEngine;

public class BossWarningUI : MonoBehaviour
{
    public static BossWarningUI Instance { get; private set; }

    [SerializeField] private GameObject warningPanel;
    [SerializeField] private TMP_Text warningText;

    private void Awake()
    {
        Instance = this;

        if (warningPanel == null)
        {
            warningPanel = gameObject;
        }

        Hide();
    }

    public void Show(string message = "WARNING")
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(true);
        }

        if (warningText != null)
        {
            warningText.text = message;
        }
    }

    public void Hide()
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }
}