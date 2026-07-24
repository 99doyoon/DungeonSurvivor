using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class GameEffectManager : MonoBehaviour
{
    public static GameEffectManager Instance
    {
        get;
        private set;
    }

    [Header("히트스톱")]
    [SerializeField]
    private float hitStopDuration = 0.15f;

    [SerializeField]
    private float slowMotionDuration = 0.3f;

    [SerializeField]
    [Range(0.01f, 1f)]
    private float slowTimeScale = 0.15f;

    private Coroutine timeEffectCoroutine;
    private bool isTimeEffectPlaying;

    [Header("연쇄 폭발 UI")]
    [SerializeField]
    private TMP_Text chainCountText;

    [SerializeField]
    private float chainResetTime = 1f;

    [SerializeField]
    private float chainTextDuration = 0.8f;

    private int chainCount;
    private float lastExplosionTime;
    private Coroutine chainHideCoroutine;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (chainCountText != null)
        {
            chainCountText.gameObject.SetActive(false);
        }
    }

    public void RegisterExplosion()
    {
        float elapsedTime =
            Time.unscaledTime -
            lastExplosionTime;

        if (elapsedTime > chainResetTime)
        {
            chainCount = 0;
        }

        chainCount++;
        lastExplosionTime = Time.unscaledTime;

        Debug.Log($"폭발 카운트: {chainCount}");

        if (chainCount < 2)
        {
            return;
        }

        ShowChainCount();
    }

    private void ShowChainCount()
    {
        if (chainCountText == null)
        {
            return;
        }

        chainCountText.gameObject.SetActive(true);
        chainCountText.text =
            $"CHAIN x{chainCount}";

        Transform textTransform =
            chainCountText.transform;

        textTransform.DOKill();
        textTransform.localScale =
            Vector3.one;

        textTransform
            .DOPunchScale(
                Vector3.one * 0.4f,
                0.25f,
                6,
                0.6f
            )
            .SetUpdate(true)
            .SetLink(
                chainCountText.gameObject,
                LinkBehaviour.KillOnDestroy
            );

        if (chainHideCoroutine != null)
        {
            StopCoroutine(
                chainHideCoroutine
            );
        }

        chainHideCoroutine =
            StartCoroutine(
                HideChainTextRoutine()
            );
    }

    private IEnumerator HideChainTextRoutine()
    {
        yield return new WaitForSecondsRealtime(
            chainTextDuration
        );

        if (chainCountText != null)
        {
            chainCountText.gameObject.SetActive(false);
        }

        chainHideCoroutine = null;
    }

    public void PlayExplosionTimeEffect()
    {
        if (isTimeEffectPlaying)
        {
            return;
        }

        timeEffectCoroutine =
            StartCoroutine(
                ExplosionTimeRoutine()
            );
    }

    private IEnumerator ExplosionTimeRoutine()
    {
        isTimeEffectPlaying = true;

        Debug.Log("히트스톱 시작");

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(
            hitStopDuration
        );

        Debug.Log("슬로모션 시작");

        Time.timeScale = slowTimeScale;

        yield return new WaitForSecondsRealtime(
            slowMotionDuration
        );

        Time.timeScale = 1f;

        Debug.Log("시간 정상화");

        isTimeEffectPlaying = false;
        timeEffectCoroutine = null;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        isTimeEffectPlaying = false;
    }
}