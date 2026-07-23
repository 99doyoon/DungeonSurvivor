using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHpUI : MonoBehaviour
{
    public static BossHpUI Instance { get; private set; }

    [SerializeField] private GameObject bossHpPanel;
    [SerializeField] private Slider bossHpSlider;
    [SerializeField] private TMP_Text bossNameText;

    [Header("체력바 애니메이션")]
    [SerializeField] private float smoothSpeed = 3f;

    private float targetValue;
    private bool isShowing;

    private void Awake()
    {
        Instance = this;

        if (bossHpPanel == null)
        {
            bossHpPanel = gameObject;
        }

        if (bossHpSlider != null)
        {
            bossHpSlider.minValue = 0f;
            bossHpSlider.maxValue = 1f;
            bossHpSlider.value = 1f;
        }

        Hide();
    }

    private void Update()
    {
        if (!isShowing || bossHpSlider == null)
        {
            return;
        }

        bossHpSlider.value = Mathf.MoveTowards(
            bossHpSlider.value,
            targetValue,
            smoothSpeed * Time.unscaledDeltaTime
        );
    }

    public void Show(
        string bossName,
        float currentHp,
        float maxHp)
    {
        if (bossHpPanel == null ||
            bossHpSlider == null)
        {
            Debug.LogError(
                "BossHpUI의 Panel 또는 Slider가 연결되지 않았습니다.",
                gameObject
            );

            return;
        }

        bossHpPanel.SetActive(true);
        isShowing = true;

        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }

        float hpRatio = GetHpRatio(currentHp, maxHp);

        bossHpSlider.value = hpRatio;
        targetValue = hpRatio;
    }

    public void SetHp(float currentHp, float maxHp)
    {
        if (!isShowing)
        {
            return;
        }

        targetValue = GetHpRatio(currentHp, maxHp);
    }

    public void Hide()
    {
        isShowing = false;
        targetValue = 0f;

        if (bossHpPanel != null)
        {
            bossHpPanel.SetActive(false);
        }
    }

    private float GetHpRatio(
        float currentHp,
        float maxHp)
    {
        if (maxHp <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp01(currentHp / maxHp);
    }
}