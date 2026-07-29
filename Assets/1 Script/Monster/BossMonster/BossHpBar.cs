using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHpBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text bossNameText;

    [Header("등장 애니메이션")]
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float moveDuration = 0.45f;
    [SerializeField] private float fillDuration = 0.7f;
    [SerializeField] private float startOffsetY = 100f;

    [Header("퇴장 애니메이션")]
    [SerializeField] private float hideDuration = 0.35f;

    private EnemyBase target;
    private Vector2 originalPosition;
    private Sequence currentSequence;
    private Coroutine showCoroutine;

    private bool isAppearing;
    private bool isHiding;

    private void Awake()
    {
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (hpSlider == null)
        {
            hpSlider = GetComponentInChildren<Slider>();
        }

        originalPosition = panelRect.anchoredPosition;

        // 처음에는 투명하게 숨긴다.
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        if (!target.gameObject.activeSelf)
        {
            Hide();
            return;
        }

        if (isAppearing)
        {
            return;
        }

        float hpRatio = GetHpRatio();

        hpSlider.value = hpRatio;
    }

    public void Show(
    EnemyBase boss,
    string bossName)
    {
        if (boss == null)
        {

            return;
        }

        if (isAppearing && target == boss)
        {

            return;
        }

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        target = boss;
        isHiding = false;
        isAppearing = true;

        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }

        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
        }

        showCoroutine =
            StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        currentSequence?.Kill();

        panelRect.DOKill();
        canvasGroup.DOKill();
        hpSlider.DOKill();

        // Canvas의 레이아웃 계산을 먼저 완료
        Canvas.ForceUpdateCanvases();

        // 애니메이션 시작 상태
        canvasGroup.alpha = 0f;

        panelRect.localScale =
            new Vector3(0f, 1f, 1f);

        hpSlider.value = 0f;

        // 시작 상태가 실제 화면에 한 번 반영되도록 대기
        yield return null;

        Canvas.ForceUpdateCanvases();

        if (target == null)
        {
            isAppearing = false;
            yield break;
        }

        float targetHpRatio = GetHpRatio();

        currentSequence = DOTween.Sequence();

        currentSequence.Append(
            canvasGroup
                .DOFade(1f, 0.5f)
                .SetEase(Ease.Linear)
        );

        currentSequence.Join(
            panelRect
                .DOScaleX(1f, 0.8f)
                .SetEase(Ease.OutCubic)
        );

        currentSequence.Append(
            hpSlider
                .DOValue(targetHpRatio, 1f)
                .SetEase(Ease.OutCubic)
        );

        currentSequence.SetUpdate(true);

        currentSequence.OnComplete(() =>
        {
            isAppearing = false;
            showCoroutine = null;

            if (target != null)
            {
                hpSlider.value = GetHpRatio();
            }
        });
    }

    public void Hide()
    {
        if (isHiding || target == null)
        {
            return;
        }

        isHiding = true;
        isAppearing = false;

        currentSequence?.Kill();
        panelRect.DOKill();
        canvasGroup.DOKill();
        hpSlider.DOKill();

        currentSequence = DOTween.Sequence();

        currentSequence.Append(
            panelRect
                .DOAnchorPos(
                    originalPosition +
                    Vector2.up * startOffsetY,
                    hideDuration
                )
                .SetEase(Ease.InBack)
        );

        currentSequence.Join(
            canvasGroup
                .DOFade(0f, hideDuration)
        );

        currentSequence.Join(
            panelRect
                .DOScale(
                    Vector3.one * 0.85f,
                    hideDuration
                )
        );

        currentSequence.SetUpdate(true);

        currentSequence.OnComplete(() =>
        {
            target = null;
            isHiding = false;

            panelRect.anchoredPosition =
                originalPosition;

            panelRect.localScale =
                Vector3.one;

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
    }

    private float GetHpRatio()
    {
        if (target == null)
        {
            return 0f;
        }

        float maxHp = target.GetMaxHp();

        if (maxHp <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp01(
            target.CurrentHp / maxHp
        );
    }

    private void OnDisable()
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }

        currentSequence?.Kill();

        panelRect?.DOKill();
        canvasGroup?.DOKill();
        hpSlider?.DOKill();

        isAppearing = false;
        isHiding = false;
    }
}