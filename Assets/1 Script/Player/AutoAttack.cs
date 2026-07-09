using System.Collections;
using System.Linq;
using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    float range;
    float AttackDelay;

    WaitForSeconds wait;

    Transform target;

    private void OnEnable()
    {
        range = 4f;
        AttackDelay = 3f;
        StartCoroutine(StartAutoAttack());
    }

    IEnumerator StartAutoAttack()
    {
        wait = new WaitForSeconds(AttackDelay);
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
        Collider2D[] detected = Physics2D.OverlapCircleAll(transform.position, range, layer);
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

    void Attack()
    {
        GameObject arr = ObjectPool.instance.GetObject(PoolType.Arrow);

        Vector2 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        arr.transform.position = transform.position;

        arr.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
