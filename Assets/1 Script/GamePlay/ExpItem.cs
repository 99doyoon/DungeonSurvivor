using DG.Tweening;
using UnityEngine;

public class ExpItem : MonoBehaviour, IPoolable
{
    [Header("자석 설정")]
    [SerializeField] private float magnetRange = 4f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 12f;

    [SerializeField] private float scatterDistance = 0.7f;
    [SerializeField] private float scatterDuration = 0.2f;

    private bool isScattering;

    private bool isCollected;

    private Transform player;
    private float currentSpeed;
    private bool isFollowing;

    private ExpManger expManger;

    [Header("경험치")]
    [SerializeField] private int expAmount = 1;

    public PoolType PoolType => PoolType.ExpItem;
    public GameObject GameObject => gameObject;

    private void OnEnable()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;

        isCollected = false;
        currentSpeed = 0f;
        isFollowing = false;
        isScattering = true;

        FindPlayer();

        Vector2 randomDirection =
            Random.insideUnitCircle.normalized;

        Vector3 targetPosition =
            transform.position +
            (Vector3)(randomDirection * scatterDistance);

        transform
            .DOMove(targetPosition, scatterDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                isScattering = false;
            });
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (!isFollowing)
        {
            float distance = Vector2.Distance(
                transform.position,
                player.position
            );

            if (distance <= magnetRange)
            {
                isFollowing = true;
            }
        }

        if (isScattering)
        {
            return;
        }

        if (isFollowing)
        {
            MoveToPlayer();
        }
    }

    public void SetExp(int amount)
    {
        expAmount = Mathf.Max(0, amount);
    }

    private void MoveToPlayer()
    {
        currentSpeed +=
            acceleration * Time.deltaTime;

        currentSpeed = Mathf.Min(
            currentSpeed,
            moveSpeed
        );

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                player.position,
                currentSpeed * Time.deltaTime
            );
    }

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindWithTag("Player");

        if (playerObject == null)
        {
            return;
        }

        player = playerObject.transform;

        expManger =
            playerObject.GetComponent<ExpManger>();

        if (expManger == null)
        {
            expManger =
                playerObject.GetComponentInChildren<ExpManger>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
        {
            return;
        }

        PlayerStatus playerStatus =
            other.GetComponentInParent<PlayerStatus>();

        if (playerStatus == null)
        {
            return;
        }

        ExpManger expManger =
            other.GetComponentInParent<ExpManger>();

        if (expManger == null)
        {
            expManger =
                playerStatus.GetComponentInChildren<ExpManger>();
        }

        if (expManger == null)
        {
            Debug.LogError(
                "Player에서 ExpManger를 찾지 못했습니다.",
                other.gameObject
            );
            return;
        }

        isCollected = true;
        isFollowing = false;

        expManger.GetExp(expAmount);

        transform.DOKill();

        transform
            .DOScale(Vector3.one * 1.4f, 0.08f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.localScale = Vector3.one;

                ObjectPool.instance.ReturnObject(this);
            });
    }
}