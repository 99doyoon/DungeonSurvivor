using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private float shakeDuration = 0.12f;
    [SerializeField] private float shakeStrength = 0.12f;

    private Coroutine shakeCoroutine;
    private Vector3 originalLocalPosition;

    private void Awake()
    {
        Instance = this;
        originalLocalPosition = transform.localPosition;
    }

    public void Play()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            transform.localPosition = originalLocalPosition;
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        originalLocalPosition = transform.localPosition;

        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.unscaledDeltaTime;

            Vector2 randomOffset =
                Random.insideUnitCircle * shakeStrength;

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

    private void OnDisable()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        transform.localPosition = originalLocalPosition;
    }
}