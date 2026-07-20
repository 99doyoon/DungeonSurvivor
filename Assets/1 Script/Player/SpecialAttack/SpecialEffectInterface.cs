using UnityEngine;

public interface IProjectileEffect
{
    bool OnHit(Bullet bullet, EnemyBase target);
}
