using DG.Tweening;
using TMPro;
using UnityEngine;

public class BossWarningUI : MonoBehaviour
{
    public static BossWarningUI Instance { get; private set; }

    [SerializeField] private GameObject warningPanel;
    [SerializeField] private TMP_Text warningText;

    private Tween blinkTween;

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    public void Show(string message)
    {
        if (warningPanel == null ||
            warningText == null)
        {
            return;
        }

        warningPanel.SetActive(true);
        warningText.text = message;
        warningText.alpha = 1f;

        blinkTween?.Kill();

        blinkTween = warningText
            .DOFade(0.2f, 0.25f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true)
            .SetLink(
                warningText.gameObject,
                LinkBehaviour.KillOnDestroy
            );
    }

    public void Hide()
    {
        blinkTween?.Kill();
        blinkTween = null;

        if (warningText != null)
        {
            warningText.DOKill();
            warningText.alpha = 1f;
        }

        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }
}