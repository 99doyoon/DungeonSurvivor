using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.2f;

    private Vector3 velocity;
    private bool canFollow = true;

    private void Update()
    {
        if (!canFollow || target == null)
        {
            return;
        }

        Vector3 targetPos = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothSpeed
        );
    }

    public void SetFollowActive(bool active)
    {
        canFollow = active;

        // 이전 SmoothDamp 속도가 남지 않도록 초기화
        velocity = Vector3.zero;
    }

    public void SnapToTarget()
    {
        if (target == null)
        {
            return;
        }

        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );

        velocity = Vector3.zero;
    }
}