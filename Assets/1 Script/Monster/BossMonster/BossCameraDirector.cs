using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BossCameraDirector : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private PlayerMoveController playerMoveController;

    [Header("카메라")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("평소 플레이어를 따라가는 카메라 스크립트")]
    [SerializeField] private CameraMove cameraFollow;

    [Header("연출 시간")]
    [SerializeField] private float moveToBossDuration = 0.7f;
    [SerializeField] private float bossViewDuration = 1.2f;
    [SerializeField] private float returnDuration = 0.7f;

    [Header("카메라 위치 보정")]
    [SerializeField] private Vector3 bossOffset = Vector3.zero;
    [SerializeField] private Vector3 playerOffset = Vector3.zero;

    private float savedTimeScale = 1f;
    private Coroutine playCoroutine;
    private Tween cameraTween;

    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null && cameraFollow == null)
        {
            cameraFollow =
                mainCamera.GetComponent<CameraMove>();
        }

        if (cameraFollow == null)
        {
            Debug.LogError(
                "Main Camera에서 CameraMove를 찾지 못했습니다.",
                gameObject
            );
        }
    }

    public void Play(
        Transform boss,
        Transform player,
        Action onBossFocused = null,
        Action onComplete = null)
    {


        if (boss == null || player == null)
        {

            onComplete?.Invoke();
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        { 
            onComplete?.Invoke();
            return;
        }

        if (isPlaying)
        {
            return;
        }

        if (playerMoveController == null)
        {
            playerMoveController =
                player.GetComponentInChildren<PlayerMoveController>();
        }

        playCoroutine = StartCoroutine(
            PlayRoutine(
                boss,
                player,
                onBossFocused,
                onComplete
            )
        );
    }

    private IEnumerator PlayRoutine(
     Transform boss,
     Transform player,
     Action onBossFocused,
     Action onComplete)
    {
        isPlaying = true;

        // 연출 시작 전 카메라 위치 저장
        Transform cameraTransform = mainCamera.transform;
        Vector3 originalCameraPosition =
            cameraTransform.position;

        // 기존 게임 시간 저장
        float previousTimeScale =
            Time.timeScale;

        // 게임 전체 정지
        Time.timeScale = 0f;

        if (playerMoveController != null)
        {
            playerMoveController.SetMoveActive(false);
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetFollowActive(false);
        }

        Vector3 bossPosition =
            new Vector3(
                boss.position.x + bossOffset.x,
                boss.position.y + bossOffset.y,
                originalCameraPosition.z
            );

        cameraTween?.Kill();

        cameraTween =
            cameraTransform
                .DOMove(
                    bossPosition,
                    moveToBossDuration
                )
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);

        yield return cameraTween.WaitForCompletion();

        onBossFocused?.Invoke();

        yield return new WaitForSecondsRealtime(
            bossViewDuration
        );

        // 플레이어 Transform이 아니라
        // 연출 시작 전 카메라 위치로 복귀
        cameraTween =
            cameraTransform
                .DOMove(
                    originalCameraPosition,
                    returnDuration
                )
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);

        yield return cameraTween.WaitForCompletion();

        if (cameraFollow != null)
        {
            cameraFollow.SetFollowActive(true);
        }

        if (playerMoveController != null)
        {
            playerMoveController.SetMoveActive(true);
        }

        // 기존 시간으로 복구
        Time.timeScale = previousTimeScale;

        isPlaying = false;
        playCoroutine = null;

        onComplete?.Invoke();
    }

    private Vector3 GetCameraPosition(
        Vector3 targetPosition,
        Vector3 offset,
        float cameraZ)
    {
        return new Vector3(
            targetPosition.x + offset.x,
            targetPosition.y + offset.y,
            cameraZ
        );
    }

    private void OnDisable()
    {
        cameraTween?.Kill();

        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }

        if (cameraFollow != null)
        {
            cameraFollow.SetFollowActive(true);
        }

        if (playerMoveController != null)
        {
            playerMoveController.SetMoveActive(true);
        }

        Time.timeScale = savedTimeScale;

        isPlaying = false;
    }
}