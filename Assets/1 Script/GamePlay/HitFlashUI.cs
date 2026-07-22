using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HitFlashUI : MonoBehaviour
{
    public static HitFlashUI Instance { get; private set; }

    [SerializeField] private Image flashImage;

    [SerializeField]
    private Color flashColor =
        new Color(1f, 0f, 0f, 0.35f);

    [SerializeField] private float flashDuration = 0.15f;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        Debug.Log($"HitFlashUI 등록: {name}", gameObject);
#endif
        if (flashImage == null)
        {
            flashImage = GetComponent<Image>();
        }

        SetAlpha(0f);
    }

    public void Play()
    {
#if UNITY_EDITOR
        Debug.Log("HitFlashUI.Play 호출됨", gameObject);
#endif
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        flashImage.color = flashColor;

        float timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.unscaledDeltaTime;

            float ratio = timer / flashDuration;
            float alpha = Mathf.Lerp(
                flashColor.a,
                0f,
                ratio
            );

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(0f);
        flashCoroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color color = flashImage.color;
        color.a = alpha;
        flashImage.color = color;
    }

    private void OnDisable()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        SetAlpha(0f);
    }
}