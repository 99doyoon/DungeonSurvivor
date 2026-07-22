using System.Collections;
using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour, IPoolable
{
    [SerializeField]
    private PoolType poolType = PoolType.EnemyDeathEffect;

    [SerializeField]
    private ParticleSystem particle;

    public PoolType PoolType => poolType;
    public GameObject GameObject => gameObject;

    private Coroutine returnCoroutine;

    private void Awake()
    {
        if (particle == null)
        {
            particle =
                GetComponentInChildren<ParticleSystem>(true);
        }

        if (particle == null)
        {
            Debug.LogError(
                $"{name}에서 ParticleSystem을 찾지 못했습니다.",
                gameObject
            );
        }
    }

    public void Play(
    Vector3 position,
    Color particleColor)
    {
        if (particle == null)
        {
            ObjectPool.instance.ReturnObject(this);
            return;
        }

        transform.position = position;

        ParticleSystem.MainModule main =
            particle.main;

        main.startColor = particleColor;

        particle.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        particle.Play();

        returnCoroutine =
            StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        yield return new WaitUntil(() =>
            particle == null ||
            !particle.IsAlive(true)
        );

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

        if (particle != null)
        {
            particle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }
}