using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;

    public Vector2 Dir { get; private set; }

    [SerializeField] PlayerStatus playerStatus;

    // Update is called once per frame
    void Update()
    {
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
        rb.linearVelocity = Dir * playerStatus.MoveSpeed;

        if(playerStatus.isDead)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
