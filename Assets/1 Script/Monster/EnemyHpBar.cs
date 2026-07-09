using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyHpBar : HpBar
{
    [SerializeField] Transform target;

    Vector3 offset = Vector3.up;

    private void LateUpdate()
    {
        if (target == null)
            return;
        transform.position = target.position + offset;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }
}