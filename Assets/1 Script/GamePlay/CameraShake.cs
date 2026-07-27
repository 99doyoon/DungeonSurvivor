using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private const string ShakeEnabledKey = "CameraShakeEnabled";
    private const string ShakeStrengthKey = "CameraShakeStrength";

    [Header("기본 흔들림 설정")]
    [SerializeField] private float shakeDuration = 0.12f;
    [SerializeField] private float shakeStrength = 0.12f;

    public bool IsShakeEnabled { get; private set; } = true;
    public float StrengthMultiplier { get; private set; } = 1f;

    private Coroutine shakeCoroutine;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        Instance = this;
        originalLocalPosition = transform.localPosition;

        LoadSettings();
    }

    public void Play()
    {
        if (!IsShakeEnabled)
        {
            return;
        }

        float finalStrength =
            shakeStrength * StrengthMultiplier;

        if (finalStrength <= 0f)
        {
            return;
        }

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition = originalLocalPosition;
        }

        shakeCoroutine = StartCoroutine(
            ShakeRoutine(
                shakeDuration,
                finalStrength
            )
        );
    }

    private IEnumerator ShakeRoutine()
    {
        float finalStrength =
            shakeStrength * StrengthMultiplier;

        yield return ShakeRoutine(
            shakeDuration,
            finalStrength
        );
    }

    private IEnumerator ShakeRoutine(
        float duration,
        float strength)
    {
        originalLocalPosition = transform.localPosition;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            Vector2 randomOffset =
                Random.insideUnitCircle * strength;

            transform.localPosition =
                originalLocalPosition +
                new Vector3(
                    randomOffset.x,
                    randomOffset.y,
                    0f
                );

            yield return null;
        }

        transform.localPosition = originalLocalPosition;
        shakeCoroutine = null;
    }

    public void SetShakeEnabled(
    bool enabled,
    bool save = true)
    {
        IsShakeEnabled = enabled;

        if (save)
        {
            PlayerPrefs.SetInt(
                "CameraShakeEnabled",
                enabled ? 1 : 0
            );

            PlayerPrefs.Save();
        }

        if (!enabled)
        {
            StopShake();
        }
    }

    public void SetStrengthMultiplier(
        float value,
        bool save = true)
    {
        StrengthMultiplier =
            Mathf.Clamp01(value);

        if (save)
        {
            PlayerPrefs.SetFloat(
                "CameraShakeStrength",
                StrengthMultiplier
            );

            PlayerPrefs.Save();
        }
    }

    private void LoadSettings()
    {
        IsShakeEnabled =
            PlayerPrefs.GetInt(
                ShakeEnabledKey,
                1
            ) == 1;

        StrengthMultiplier =
            PlayerPrefs.GetFloat(
                ShakeStrengthKey,
                1f
            );
    }

    private void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        transform.localPosition =
            originalLocalPosition;
    }

    private void OnDisable()
    {
        StopShake();
    }
}