using UnityEngine;

public class EffectAutoDestroy : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 0.5f;

    private void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }
}