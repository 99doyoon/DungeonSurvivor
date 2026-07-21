using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimationController : ChracterAnimation
{
    [SerializeField] private PlayerMoveController playerMoveController;

    private void Awake()
    {
        SettingAnimation();
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
}