using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private RectTransform rectTransform;

    [Header("등장 애니메이션")]
    [SerializeField] private float appearDuration = 0.35f;
    [SerializeField] private float startOffsetY = -150f;
    [SerializeField] private float startScale = 0.5f;

    [Header("마우스 애니메이션")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float hoverDuration = 0.15f;

    [Header("UI")]
    [SerializeField] private Button cardButton;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text rewardNameText;
    [SerializeField] private TMP_Text descriptionText;

    private bool isSelected;
    private Vector2 originalPosition;
    private Tween currentTween;

    private RewardData rewardData;
    private Action<RewardData> selectAction;

    private void Awake()
    {
        if (cardButton == null)
        {
            cardButton = GetComponent<Button>();
        }

        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        originalPosition = rectTransform.anchoredPosition;
    }

    public void SetReward(
        RewardData data,
        Action<RewardData> action)
    {
        isSelected = false;
        cardButton.interactable = true;

        rewardData = data;
        selectAction = action;

        rewardNameText.text = data.rewardName;
        descriptionText.text = data.description;
        iconImage.sprite = data.icon;

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(SelectReward);
    }

    private void SelectReward()
    {
        if (isSelected)
        {
            return;
        }

        if (rewardData == null || selectAction == null)
        {
            return;
        }

        isSelected = true;
        cardButton.interactable = false;

        PlaySelectAnimation(() =>
        {
            selectAction.Invoke(rewardData);
        });
    }

    private void PlaySelectAnimation(Action onComplete)
    {
        currentTween?.Kill();
        rectTransform.DOKill();

        Sequence sequence = DOTween.Sequence();

        // 먼저 살짝 커짐
        sequence.Append(
            rectTransform
                .DOScale(Vector3.one * 1.2f, 0.12f)
                .SetEase(Ease.OutQuad)
        );

        // 이후 작아지면서 사라짐
        sequence.Append(
            rectTransform
                .DOScale(Vector3.zero, 0.2f)
                .SetEase(Ease.InBack)
        );

        // Time.timeScale이 0이어도 실행
        sequence.SetUpdate(true);

        sequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });

        currentTween = sequence;
    }

    public void PlayAppearAnimation(float delay)
    {
        currentTween?.Kill();
        rectTransform.DOKill();

        rectTransform.anchoredPosition =
            originalPosition + Vector2.up * startOffsetY;

        rectTransform.localScale =
            Vector3.one * startScale;

        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(delay);

        sequence.Append(
            rectTransform
                .DOAnchorPos(originalPosition, appearDuration)
                .SetEase(Ease.OutBack)
        );

        sequence.Join(
            rectTransform
                .DOScale(Vector3.one, appearDuration)
                .SetEase(Ease.OutBack)
        );

        sequence.SetUpdate(true);

        currentTween = sequence;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOKill();

        rectTransform
            .DOScale(Vector3.one * hoverScale, hoverDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOKill();

        rectTransform
            .DOScale(Vector3.one, hoverDuration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    private void OnDisable()
    {
        currentTween?.Kill();
        rectTransform?.DOKill();
    }
}