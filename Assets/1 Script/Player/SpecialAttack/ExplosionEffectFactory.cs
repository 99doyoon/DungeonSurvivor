using UnityEngine;

public class ExplosionEffectFactory : IProjectileEffectFactory
{
    private int explosionLevel;

    private readonly float baseRadius;
    private readonly float damageRatio;
    private readonly PoolType effectPoolType;

    public ExplosionEffectFactory(
        float baseRadius,
        float damageRatio,
        PoolType effectPoolType)
    {
        this.baseRadius = baseRadius;
        this.damageRatio = damageRatio;
        this.effectPoolType = effectPoolType;

        explosionLevel = 1;
    }

    public void LevelUp()
    {
        explosionLevel++;
    }

    public IProjectileEffect CreateEffect(float playerDamage)
    {
        float radius =
            baseRadius + (explosionLevel - 1) * 0.2f;

        float damage =
            playerDamage * damageRatio;

        return new ExplosionEffect(
            radius,
            damage,
            effectPoolType
        );
    }
}