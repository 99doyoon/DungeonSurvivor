using System.Collections.Generic;
using UnityEngine;

public class ProjectileEffectManager : MonoBehaviour
{
    private bool hasExplosion;
    private int explosionLevel;

    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionDamageRatio = 0.5f;
    [SerializeField]
    private PoolType explosionEffectPoolType = PoolType.ExplosionEffect;

    public void AddExplosion()
    {
        hasExplosion = true;
        explosionLevel++;

#if UNITY_EDITOR
        Debug.Log($"폭발 투사체 획득: {explosionLevel}레벨");
#endif
    }

    public List<IProjectileEffect> CreateEffects(float playerDamage)
    {
        List<IProjectileEffect> effects =
            new List<IProjectileEffect>();

        if (hasExplosion)
        {
            float radius =
                explosionRadius + (explosionLevel - 1) * 0.2f;

            float explosionDamage =
                playerDamage * explosionDamageRatio;

            effects.Add(new ExplosionEffect(radius,explosionDamage,explosionEffectPoolType));
        }

        return effects;
    }
}