using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private float smoothSpeed = 80f;

    private float targetHp;
    private bool isInitialized;

    private void Awake()
    {
        if (hpSlider == null)
        {
            hpSlider = GetComponent<Slider>();
        }
    }

    public void Init(float maxHp, float currentHp)
    {
        if (hpSlider == null)
        {
            Debug.LogError("PlayerHpBar에 Slider가 없습니다.", gameObject);
            return;
        }

        hpSlider.minValue = 0f;
        hpSlider.maxValue = maxHp;

        hpSlider.value = currentHp;
        targetHp = currentHp;

        isInitialized = true;
    }

    public void SetHp(float currentHp)
    {
        if (!isInitialized)
        {
            return;
        }

        targetHp = Mathf.Clamp(
            currentHp,
            hpSlider.minValue,
            hpSlider.maxValue
        );
    }

    public void SetMaxHp(float maxHp, float currentHp)
    {
        if (!isInitialized)
        {
            Init(maxHp, currentHp);
            return;
        }

        hpSlider.maxValue = maxHp;
        targetHp = Mathf.Clamp(currentHp, 0f, maxHp);
    }

    private void Update()
    {
        if (!isInitialized)
        {
            return;
        }

        hpSlider.value = Mathf.MoveTowards(
            hpSlider.value,
            targetHp,
            smoothSpeed * Time.unscaledDeltaTime
        );
    }
}