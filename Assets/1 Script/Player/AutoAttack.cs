using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerStatus))]
public class AutoAttack : MonoBehaviour
{
    [SerializeField]
    private ProjectileEffectManager projectileEffectManager;

    WaitForSeconds wait;

    Transform target;

    private PlayerStatus playerStatus;

    private void Awake()
    {
        playerStatus = GetComponentInParent<PlayerStatus>();

        playerStatus = GetComponentInParent<PlayerStatus>();
        projectileEffectManager =
            GetComponentInParent<ProjectileEffectManager>();
#if UNITY_EDITOR
        if (playerStatus == null)
        {
            Debug.LogError("AutoAttack이 PlayerStatus를 찾지 못했습니다.");
        }
        Debug.Log(
            $"[AutoAttack] PlayerStatus: {playerStatus.gameObject.name}, " +
            $"ID: {playerStatus.GetInstanceID()}");

        if (projectileEffectManager == null)
        {
            Debug.LogError(
                "AutoAttack이 ProjectileEffectManager를 찾지 못했습니다."
            );
        }
#endif
    }

    private void OnEnable()
    {

        StartCoroutine(StartAutoAttack());
    }

    IEnumerator StartAutoAttack()
    {
        wait = new WaitForSeconds(playerStatus.AttackDelay);
        while (true)
        {
            yield return new WaitUntil(() => ScanEnemy());
            Attack();
            yield return wait;
        }
    }
    bool ScanEnemy()
    {
        int EnemyLayerMask = LayerMask.NameToLayer("Enemy");
        int layer = 1 << EnemyLayerMask;
        Collider2D[] detected = Physics2D.OverlapCircleAll(transform.position, playerStatus.AttackRange, layer);
        if (detected.Length <= 0)
        {
            return false;
        }
        else
        {
            Collider2D targetCollider2D = detected.OrderBy(col => Vector2.Distance(transform.position, col.transform.position)).FirstOrDefault();
            target = targetCollider2D.transform;
            return true;
        }
    }

    public virtual void Attack()
    {
        if (target == null)
        {
            return;
        }

        //화살발사 사운드
        SoundManager.Instance?.PlaySfx(
            SFXType.PlayerAttack
        );

        int projectileCount = Mathf.Max(1, playerStatus.ProjectileCount);
        Vector2 dir = target.position - transform.position;
        dir.Normalize();
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float angleGap = 15f;
        float startAngle = -(projectileCount - 1) * angleGap * 0.5f;

        for (int i = 0; i < projectileCount; i++)
        {

            Arrow arrow =
                ObjectPool.instance.GetObject<Arrow>(PoolType.Arrow);

            float shotAngle =
                baseAngle + startAngle + angleGap * i;

            arrow.transform.position = transform.position;
            arrow.transform.rotation =
                Quaternion.Euler(0f, 0f, shotAngle);

            List<IProjectileEffect> effects = projectileEffectManager.CreateEffects(playerStatus.Damage);

            arrow.Init(
                playerStatus.Damage,
                playerStatus.ProjectileSpeed,
                effects
            );

        }
    }
}
