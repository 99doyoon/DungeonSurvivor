//using DG.Tweening;
//using TMPro;
//using UnityEngine;

//public class DamageText : MonoBehaviour, IPoolable
//{
//    [SerializeField] private PoolType poolType = PoolType.DamageText;
//    [SerializeField] private TMP_Text damageText;

//    [SerializeField] private float moveDistance = 0.8f;
//    [SerializeField] private float duration = 0.6f;

//    [SerializeField] private Canvas worldCanvas;

//    public PoolType PoolType => poolType;
//    public GameObject GameObject => gameObject;

//    private Sequence sequence;

//    private Vector3 originalScale;

//    private void Awake()
//    {
//        originalScale = transform.localScale;

//        if (damageText == null)
//        {
//            damageText = GetComponentInChildren<TMP_Text>(true);
//        }

//        if (worldCanvas == null)
//        {
//            worldCanvas = GetComponentInChildren<Canvas>(true);
//        }

//        if (worldCanvas != null)
//        {
//            worldCanvas.renderMode = RenderMode.WorldSpace;
//            worldCanvas.overrideSorting = true;
//            worldCanvas.sortingOrder = 200;
//        }
//    }

//    public void Play(Vector3 position, float damage)
//    {
//#if UNITY_EDITOR
//        Debug.Log($"DamageText.Play 실행: {damage}", gameObject);
//#endif

//        //transform.position = position;

//        //damageText.text =
//        //    Mathf.RoundToInt(damage).ToString();

//        //damageText.alpha = 1f;
//        //transform.localScale = Vector3.one;

//        //sequence?.Kill();

//        //sequence = DOTween.Sequence();

//        //sequence.Append(
//        //    transform.DOMoveY(
//        //        position.y + moveDistance,
//        //        duration
//        //    )
//        //);

//        //sequence.Join(
//        //    damageText.DOFade(0f, duration)
//        //);

//        //sequence.OnComplete(() =>
//        //{
//        //    sequence = null;
//        //    ObjectPool.instance.ReturnObject(this);
//        //});

//        transform.SetParent(null);

//        transform.position = new Vector3(
//            Camera.main.transform.position.x,
//            Camera.main.transform.position.y,
//            0f
//        );

//        transform.localScale = Vector3.one;

//        damageText.text = "999";
//        damageText.color = Color.red;
//        damageText.alpha = 1f;

//        if (worldCanvas != null)
//        {
//            worldCanvas.transform.localScale =
//                Vector3.one * 0.01f;

//            worldCanvas.overrideSorting = true;
//            worldCanvas.sortingOrder = 500;
//        }

//        Debug.Log("카메라 중앙 강제 표시 테스트", gameObject);

//        Debug.Log(
//            $"DamageText 표시 시도: {damageText.text}, " +
//            $"위치: {transform.position}",
//            gameObject
//        );
//        Debug.Log(
//            $"활성: {gameObject.activeInHierarchy}, " +
//            $"위치: {transform.position}, " +
//            $"스케일: {transform.lossyScale}, " +
//            $"텍스트: {damageText.text}, " +
//            $"알파: {damageText.alpha}",
//            gameObject
//        );
//        Debug.Log(
//            $"부모: {(transform.parent == null ? "없음" : transform.parent.name)}, " +
//            $"LocalScale: {transform.localScale}, " +
//            $"LossyScale: {transform.lossyScale}"
//        );
//    }

//    private void OnDisable()
//    {
//        sequence?.Kill();
//        sequence = null;

//        if (damageText != null)
//        {
//            damageText.alpha = 1f;
//        }

//        transform.localScale = originalScale;
//        //transform.localScale = Vector3.one;
//    }
//}

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
            Debug.LogError(
                $"{name}에서 TextMeshPro를 찾지 못했습니다.",
                this
            );
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