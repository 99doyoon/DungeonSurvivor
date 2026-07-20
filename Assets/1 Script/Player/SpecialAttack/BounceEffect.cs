using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BounceEffect : IProjectileEffect
{
    private int remainBounceCount;
    private float searchRadius;

    private readonly HashSet<EnemyBase> hitEnemies
        = new HashSet<EnemyBase>();

    public BounceEffect(
        int bounceCount,
        float searchRadius)
    {
        // 남은 횟수 저장
        remainBounceCount = bounceCount;
        // 탐색 범위 저장
        this.searchRadius = searchRadius;
    }

    public bool OnHit(
        Bullet bullet,
        EnemyBase target)
    {
        // 1. 현재 적을 hitEnemies에 추가
        hitEnemies.Add(target);
        // 2. 남은 횟수가 있는지 확인
        if (remainBounceCount <= 0)
        {
#if UNITY_EDITOR
            Debug.Log("남은 튕김 횟수 없음");
#endif
            return false;
        }
        // 3. 주변 Collider 탐색
        EnemyBase nextTarget = FindNextTarget(target);

        // 4. 가장 가까운 새로운 적 찾기
        if (nextTarget == null)
        {
#if UNITY_EDITOR
            Debug.Log("다음 적을 찾지 못함");
#endif
            return false;
        }

        // 5. 다음 적이 없다면 종료

        //화살발사 사운드
        SoundManager.Instance?.PlaySfx(
            SFXType.PlayerAttack
        );

        // 6. 방향 계산
        Vector2 dir = nextTarget.transform.position-target.transform.position;

        // 7. arrow.ChangeDirection 호출
        bullet.ChangeDirection(dir);

        // 8. 남은 횟수 감소
        remainBounceCount--;
#if UNITY_EDITOR
        Debug.Log(
       $"{target.name}에서 {nextTarget.name}으로 튕김"
        );
#endif
        // 9. 화살이 풀로 반환되지 않게 표시
        return true;
    }

    private EnemyBase FindNextTarget(EnemyBase currentTarget)
    {
        int enemyLayer = LayerMask.GetMask("Enemy");

        //거리 비교용 변수
        EnemyBase nextTarget = null;
        float nearestDistance = float.MaxValue;

        Collider2D[] detected = Physics2D.OverlapCircleAll(currentTarget.transform.position,searchRadius,enemyLayer);

        foreach (Collider2D collider in detected)
        {
            EnemyBase candidate =
                collider.GetComponent<EnemyBase>();

            if (candidate == null)
            {
                continue;
            }

            if (candidate == currentTarget)
            {
                continue;
            }

            if (hitEnemies.Contains(candidate))
            {
                continue;
            }

            // 여기까지 왔다면 다음 적 후보
            float distance = Vector2.Distance(currentTarget.transform.position, candidate.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nextTarget = candidate;
            }
        }

        if (nextTarget != null)
        {
            hitEnemies.Add(nextTarget);
        }

        return nextTarget;
    }
}