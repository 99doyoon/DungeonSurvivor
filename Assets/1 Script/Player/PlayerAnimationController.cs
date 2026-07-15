using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : ChracterAnimation
{
    [SerializeField] private PlayerMoveController playerMoveController;

    private readonly int isHitHash = Animator.StringToHash("isHit");

    private Color originColor;
    private Sequence hitSequence;

    private void Awake()
    {
        SettingAnimation();

        if (sr != null)
        {
            originColor = sr.color;
        }
    }

    private void Update()
    {
        CheckFlip();
        CheckMove();
    }

    public override void CheckFlip()
    {
        if (sr == null || Camera.main == null || Mouse.current == null)
        {
            return;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        sr.flipX = transform.position.x >= worldPos.x;
    }

    public override void CheckMove()
    {
        if (anim == null || playerMoveController == null)
        {
            return;
        }

        anim.SetBool(
            isMoveHash,
            playerMoveController.Dir != Vector2.zero
        );
    }

    public void HitAnimation()
    {
        if (anim == null || sr == null)
        {
            return;
        }

        anim.SetTrigger(isHitHash);

        // 기존 피격 색상 Tween이 남아 있다면 제거
        hitSequence?.Kill();

        sr.color = originColor;

        hitSequence = DOTween.Sequence()
            .Append(sr.DOColor(Color.red, 0.15f))
            .Append(sr.DOColor(originColor, 0.15f))
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    private void OnDisable()
    {
        hitSequence?.Kill();
        hitSequence = null;

        if (sr != null)
        {
            sr.color = originColor;
        }
    }

    private void OnDestroy()
    {
        hitSequence?.Kill();
        hitSequence = null;
    }
}