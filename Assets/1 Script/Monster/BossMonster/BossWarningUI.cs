using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BossWarningUI : MonoBehaviour
{
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text warningText;

    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float warningDuration = 1.5f;

    private bool isPlaying;

    private void Awake()
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }

    public void Play(Action onComplete)
    {

        if (isPlaying)
        {

            return;
        }

        StartCoroutine(PlayRoutine(onComplete));
    }

    private IEnumerator PlayRoutine(Action onComplete)
    {

        isPlaying = true;
        Time.timeScale = 0f;

        warningPanel.SetActive(true);
        warningPanel.transform.SetAsLastSibling();

        canvasGroup.alpha = 0f;

        warningText.text = "WARNING";
        warningText.rectTransform.localScale = Vector3.one * 0.5f;

        Sequence appearSequence = DOTween.Sequence();

        appearSequence.Append(
            canvasGroup.DOFade(1f, fadeDuration)
        );

        appearSequence.Join(
            warningText.rectTransform
                .DOScale(Vector3.one, fadeDuration)
                .SetEase(Ease.OutBack)
        );

        appearSequence.SetUpdate(true);

        yield return new WaitForSecondsRealtime(warningDuration);

        canvasGroup
            .DOFade(0f, fadeDuration)
            .SetUpdate(true);

        yield return new WaitForSecondsRealtime(fadeDuration);

        warningPanel.SetActive(false);

        Time.timeScale = 1f;
        isPlaying = false;

        onComplete?.Invoke();
    }

    private void OnDisable()
    {
        canvasGroup?.DOKill();

        if (warningText != null)
        {
            warningText.rectTransform.DOKill();
        }

        isPlaying = false;
    }
}