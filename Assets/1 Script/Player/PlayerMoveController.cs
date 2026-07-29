using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerStatus playerStatus;

    public Vector2 Dir { get; private set; }

    private bool canMove = true;

    private void Update()
    {
        if (!canMove || playerStatus.isDead)
        {
            Dir = Vector2.zero;
            return;
        }

        Dir = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
        {
            Dir += Vector2.up;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            Dir += Vector2.left;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            Dir += Vector2.down;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            Dir += Vector2.right;
        }

        Dir = Dir.normalized;
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            return;
        }

        if (!canMove || playerStatus.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity =
            Dir * playerStatus.MoveSpeed;
    }

    public void SetMoveActive(bool active)
    {
        canMove = active;
        Dir = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}