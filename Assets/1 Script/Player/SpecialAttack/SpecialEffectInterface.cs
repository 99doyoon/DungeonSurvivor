using UnityEngine;

public interface IProjectileEffect
{
    void OnHit(Bullet bullet, EnemyBase target);
}
