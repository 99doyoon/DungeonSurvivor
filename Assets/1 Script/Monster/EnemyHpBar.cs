using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class EnemyHpBar : HpBar, IPoolable
{
    [Header("등장 애니메이션")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float appearDuration = 0.6f;

    [SerializeField] private float startOffsetY = 80f;

    private bool isPlayingAppearAnimation;

    private Vector2 originalPosition;

    private EnemyBase target;
    private Camera mainCam;

    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    public PoolType PoolType => PoolType.EnemyHpBar;
    public GameObject GameObject => gameObject;

    private void Awake()
    {
        mainCam = Camera.main;

        if (hpSlider == null)
        {
            hpSlider = GetComponent<Slider>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void LateUpdate()
    {
        if (target == null || !target.gameObject.activeSelf)
        {
            target = null;
            ObjectPool.instance.ReturnObject(this);
            return;
        }

        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        Vector3 worldPos =
            target.transform.position + offset;

        transform.position =
            mainCam.WorldToScreenPoint(worldPos);

        if (!isPlayingAppearAnimation)
        {
            SetGage(
                target.CurrentHp / target.GetMaxHp()
            );
        }
    }

    public void SetTarget(EnemyBase monster)
    {
        hpSlider?.DOKill();
        canvasGroup?.DOKill();

        isPlayingAppearAnimation = false;
        target = monster;

        if (target == null)
        {
            return;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        SetGage(
            target.CurrentHp / target.GetMaxHp()
        );
    }

    public void PlayBossAppearAnimation()
    {
        if (hpSlider == null || canvasGroup == null || target == null)
        {
            return;
        }

        hpSlider.DOKill();
        canvasGroup.DOKill();

        isPlayingAppearAnimation = true;

        float targetValue =
            Mathf.Clamp01(
                target.CurrentHp / target.GetMaxHp()
            );

        canvasGroup.alpha = 0f;
        hpSlider.value = 0f;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            canvasGroup
                .DOFade(1f, 0.2f)
        );

        sequence.Append(
            hpSlider
                .DOValue(targetValue, appearDuration)
                .SetEase(Ease.OutCubic)
        );

        sequence.SetUpdate(true);

        sequence.OnComplete(() =>
        {
            isPlayingAppearAnimation = false;

            SetGage(
                target.CurrentHp / target.GetMaxHp()
            );
        });
    }
}