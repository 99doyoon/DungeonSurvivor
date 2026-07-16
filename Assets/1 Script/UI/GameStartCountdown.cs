using System.Collections;
using TMPro;
using UnityEngine;

public class GameStartCountdown : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text countdownText;

    [Header("카운트다운 설정")]
    [SerializeField] private float startDelay = 0.8f;
    [SerializeField] private int startCount = 3;
    [SerializeField] private float countInterval = 1f;
    [SerializeField] private float startTextDuration = 1.2f;

    [Header("효과음")]
    [SerializeField] private bool useSound = true;

    private Coroutine countdownCoroutine;

    private void Start()
    {
        // 이전 게임오버 등으로 시간이 멈춰 있을 수 있으므로
        // 게임 씬 시작 시 카운트다운 상태로 초기화한다.
        Time.timeScale = 0f;

        countdownCoroutine =
            StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        if (countdownText == null)
        {
            Debug.LogError(
                "GameStartCountdown에 Countdown Text가 연결되지 않았습니다.",
                gameObject
            );

            Time.timeScale = 1f;
            yield break;
        }

        countdownText.gameObject.SetActive(true);

        // 게임 씬이 열린 직후 잠시 쉬었다가 3부터 시작한다.
        countdownText.text = "";

        yield return new WaitForSecondsRealtime(startDelay);

        for (int count = startCount; count >= 1; count--)
        {
            countdownText.text = count.ToString();

            countdownText.transform.localScale =
                Vector3.one * 1.3f;

            if (useSound)
            {
                SoundManager.Instance?.PlaySfx(
                    SFXType.Countdown
                );
            }

            yield return ScaleTextRoutine(countInterval);
        }

        countdownText.text = "START!";

        countdownText.transform.localScale =
            Vector3.one * 1.3f;

        if (useSound)
        {
            SoundManager.Instance?.PlaySfx(
                SFXType.GameStart
            );
        }

        yield return ScaleTextRoutine(startTextDuration);

        countdownText.gameObject.SetActive(false);

        Time.timeScale = 1f;

        countdownCoroutine = null;
    }

    /// <summary>
    /// 카운트다운 글자가 조금씩 작아지는 간단한 연출.
    /// timeScale이 0이므로 unscaledDeltaTime을 사용한다.
    /// </summary>
    private IEnumerator ScaleTextRoutine(float duration)
    {
        float timer = 0f;

        Vector3 startScale = Vector3.one * 1.3f;
        Vector3 targetScale = Vector3.one;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float ratio =
                Mathf.Clamp01(timer / duration);

            countdownText.transform.localScale =
                Vector3.Lerp(
                    startScale,
                    targetScale,
                    ratio
                );

            yield return null;
        }

        countdownText.transform.localScale =
            targetScale;
    }

    private void OnDestroy()
    {
        // 카운트다운 도중 다른 씬으로 이동해도
        // 다음 씬이 멈춘 상태로 시작하지 않도록 복구한다.
        if (countdownCoroutine != null)
        {
            Time.timeScale = 1f;
        }
    }
}