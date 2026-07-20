using System.Collections;
using System.Drawing;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class EffectObject : MonoBehaviour, IPoolable
{
    [SerializeField]
    private PoolType poolType =
        PoolType.ExplosionEffect;

    [SerializeField]
    private float lifeTime = 0.5f;

    //이펙트를 범위에 맞게 설정하기위해 가져오는 컴포넌트
    [SerializeField]
    private ParticleSystem particle;

    //이펙트 사이즈에 보정값을 넣기위한 변수
    [SerializeField]
    private float particleSizeMultiplier = 1f;

    //폭발 반지름
    private float currentRadius;

    public PoolType PoolType => poolType;

    public GameObject GameObject => gameObject;

    private Coroutine returnCoroutine;

    public void Play(Vector3 position, float radius)
    {
        transform.position = position;
        transform.rotation = Quaternion.identity;

        currentRadius = radius;

        if (particle != null)
        {
            ParticleSystem.MainModule main = particle.main;

            // 판정은 반지름, 시각 크기는 지름으로 맞춤
            main.startSize = radius * 2f * particleSizeMultiplier;

            particle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            particle.Play();
        }

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        returnCoroutine = StartCoroutine(ReturnAfterTime());
    }

    private IEnumerator ReturnAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);

        returnCoroutine = null;

        ObjectPool.instance.ReturnObject(this);
    }

    private void OnDisable()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            currentRadius
        );
    }
}