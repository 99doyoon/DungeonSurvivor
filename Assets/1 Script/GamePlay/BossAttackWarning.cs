using DG.Tweening;
using UnityEngine;

public class BossAttackWarning : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer warningRenderer;

    private Tween warningTween;

    private void Awake()
    {
        if (warningRenderer == null)
        {
            warningRenderer =
                GetComponentInChildren<SpriteRenderer>(true);
        }
    }

    public void Show(
        Vector3 position,
        float radius,
        float warningTime)
    {
        warningTween?.Kill();

        transform.SetParent(null);

        transform.position = new Vector3(
            position.x,
            position.y,
            0f
        );

        transform.rotation = Quaternion.identity;

        float diameter = radius * 2f;

        transform.localScale = new Vector3(
            diameter,
            diameter,
            1f
        );

        gameObject.SetActive(true);

        Color color = warningRenderer.color;
        color.a = 0.25f;
        warningRenderer.color = color;

        warningTween = warningRenderer
            .DOFade(0.75f, warningTime)
            .SetEase(Ease.InQuad);

        Debug.Log(
            $"경고 위치: {position}, " +
            $"반지름: {radius}, " +
            $"지름: {diameter}"
        );
    }

    public void Hide()
    {
        warningTween?.Kill();
        warningTween = null;

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        warningTween?.Kill();
        warningTween = null;
    }
}