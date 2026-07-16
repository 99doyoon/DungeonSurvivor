using System.Collections;
using UnityEngine;

public abstract class ChracterAnimation : MonoBehaviour
{
    protected Animator anim;
    protected SpriteRenderer sr;
    protected Camera camera;

    protected readonly int isMoveHash =
        Animator.StringToHash("isMove");

    [Header("사망 연출")]
    [SerializeField] private float fallAngle = 90f;
    [SerializeField] private float fallDuration = 0.3f;

    // SpriteRenderer가 붙어 있는 실제 그림 오브젝트
    private Transform visualTransform;

    // 풀에서 다시 활성화될 때 복원할 원래 회전값
    private Quaternion originalRotation;

    private Coroutine fallCoroutine;

    public abstract void CheckFlip();

    public abstract void CheckMove();

    public virtual void SettingAnimation()
    {
        camera = Camera.main;
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError(
                $"{gameObject.name}: SpriteRenderer를 찾지 못했습니다.",
                gameObject
            );

            return;
        }

        visualTransform = sr.transform;
        originalRotation = visualTransform.localRotation;
    }

    /// <summary>
    /// 캐릭터의 그림 오브젝트를 회전시켜
    /// 옆으로 쓰러지는 모습을 표현한다.
    /// </summary>
    public virtual void PlayFall()
    {
        if (visualTransform == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: SettingAnimation이 호출되지 않았거나 " +
                "SpriteRenderer가 없습니다.",
                gameObject
            );

            return;
        }

        if (fallCoroutine != null)
        {
            StopCoroutine(fallCoroutine);
        }

        fallCoroutine = StartCoroutine(FallRoutine());
    }

    /// <summary>
    /// 지정한 방향으로 쓰러진다.
    /// direction이 1이면 한쪽, -1이면 반대쪽으로 쓰러진다.
    /// </summary>
    public virtual void PlayFall(float direction)
    {
        if (visualTransform == null)
            return;

        if (fallCoroutine != null)
        {
            StopCoroutine(fallCoroutine);
        }

        fallCoroutine =
            StartCoroutine(FallRoutine(direction));
    }

    private IEnumerator FallRoutine()
    {
        // 왼쪽 또는 오른쪽으로 무작위로 쓰러진다.
        float direction =
            Random.value < 0.5f ? -1f : 1f;

        yield return FallRoutine(direction);
    }

    private IEnumerator FallRoutine(float direction)
    {
        Quaternion startRotation =
            visualTransform.localRotation;

        Quaternion targetRotation =
            originalRotation *
            Quaternion.Euler(
                0f,
                0f,
                fallAngle * direction
            );

        float timer = 0f;

        while (timer < fallDuration)
        {
            timer += Time.deltaTime;

            float ratio =
                Mathf.Clamp01(timer / fallDuration);

            // 처음과 마지막을 부드럽게 만든다.
            float smoothRatio =
                Mathf.SmoothStep(0f, 1f, ratio);

            visualTransform.localRotation =
                Quaternion.Lerp(
                    startRotation,
                    targetRotation,
                    smoothRatio
                );

            yield return null;
        }

        visualTransform.localRotation =
            targetRotation;

        fallCoroutine = null;
    }

    /// <summary>
    /// 오브젝트 풀에서 다시 활성화될 때
    /// 회전값을 원래 상태로 되돌린다.
    /// </summary>
    public virtual void ResetFall()
    {
        if (fallCoroutine != null)
        {
            StopCoroutine(fallCoroutine);
            fallCoroutine = null;
        }

        if (visualTransform != null)
        {
            visualTransform.localRotation =
                originalRotation;
        }
    }
}