using UnityEngine;

public class BounceEffectFactory : IProjectileEffectFactory
{
    private int bounceLevel;
    private readonly int baseBounceCount;
    private readonly float searchRadius;

    public BounceEffectFactory(
        int baseBounceCount,
        float searchRadius)
    {
        this.baseBounceCount = baseBounceCount;
        this.searchRadius = searchRadius;

        bounceLevel = 1;
    }

    public void LevelUp()
    {
        bounceLevel++;
    }

    public IProjectileEffect CreateEffect(float playerDamage)
    {
        int bounceCount =
            baseBounceCount + bounceLevel - 1;

        return new BounceEffect(
            bounceCount,
            searchRadius
        );
    }
}