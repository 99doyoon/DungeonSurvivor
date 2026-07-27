using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour, IPoolable
{
    [SerializeField]
    private PoolType poolType = PoolType.DamageText;

    [SerializeField]
    private TextMeshPro damageText;

    [SerializeField]
    private float moveDistance = 0.8f;

    [SerializeField]
    private float duration = 0.6f;

    public PoolType PoolType => poolType;
    public GameObject GameObject => gameObject;

    private Sequence sequence;

    private void Awake()
    {
        if (damageText == null)
        {
            damageText = GetComponent<TextMeshPro>();
        }

        if (damageText == null)
        {

        }
    }

    public void Play(Vector3 position, float damage)
    {
        sequence?.Kill();

        transform.position = new Vector3(
            position.x,
            position.y,
            -1f
        );

        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        damageText.text =
            Mathf.RoundToInt(damage).ToString();

        damageText.color = Color.red;
        damageText.alpha = 1f;

        sequence = DOTween.Sequence();

        sequence.Join(
            transform.DOMoveY(
                transform.position.y + moveDistance,
                duration
            )
        );

        sequence.Join(
            damageText.DOFade(0f, duration)
        );

        sequence.OnComplete(() =>
        {
            sequence = null;
            ObjectPool.instance.ReturnObject(this);
        });
    }

    private void OnDisable()
    {
        sequence?.Kill();
        sequence = null;

        if (damageText != null)
        {
            damageText.alpha = 1f;
        }
    }
}