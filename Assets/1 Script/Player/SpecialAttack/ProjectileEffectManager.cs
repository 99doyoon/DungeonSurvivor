using System.Collections.Generic;
using UnityEngine;

//이 인터페이스를 구현한 클래스는 투사체 효과를 하나 생성할 책임을 가져.
public interface IProjectileEffectFactory
{
    IProjectileEffect CreateEffect(float playerDamage);
}

public class ProjectileEffectManager : MonoBehaviour
{
    [Header("특수기능 관리 리스트")]
    private readonly List<IProjectileEffectFactory> factories
        = new List<IProjectileEffectFactory>();

    [Header("폭발화살")]
    private ExplosionEffectFactory explosionFactory;
    [SerializeField]
    private int baseExplosionCount = 1;
    private readonly float baseRadius=2f;
    private readonly float damageRatio=0.5f;
    private readonly PoolType effectPoolType = PoolType.ExplosionEffect;

    [Header("팅김화살")]
    private BounceEffectFactory bounceFactory;
    [SerializeField]
    private int baseBounceCount = 1;
    [SerializeField]
    private float bounceSearchRadius = 5f;

    //폭발기능추가
    public void AddExplosion()
    {
        if(explosionFactory == null)
        {
            explosionFactory = new ExplosionEffectFactory(baseRadius, damageRatio, effectPoolType);
            factories.Add(explosionFactory);
            return;
        }
        explosionFactory.LevelUp();
    }

    public List<IProjectileEffect> CreateEffects(float playerDamage)
    {
        List<IProjectileEffect> effects =
            new List<IProjectileEffect>();

        foreach (IProjectileEffectFactory factory in factories)
        {
            effects.Add(
                factory.CreateEffect(playerDamage)
            );
        }

        return effects;
    }

    //팅김기능추가
    public void AddBounce()
    {
        if (bounceFactory == null)
        {
            bounceFactory = new BounceEffectFactory(baseBounceCount,bounceSearchRadius);

            factories.Add(bounceFactory);
            return;
        }

        bounceFactory.LevelUp();
    }
}