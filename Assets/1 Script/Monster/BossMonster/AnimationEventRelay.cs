using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    // 부모 보스의 행동 스크립트
    private BossMonster bossMonster;

    private void Awake()
    {
        // Animator 자식에서 부모 방향으로 BossMonster를 찾는다.
        bossMonster = GetComponentInParent<BossMonster>();

        if (bossMonster == null)
        {
            Debug.LogError(
                $"{gameObject.name}의 부모에서 BossMonster를 찾을 수 없습니다.",
                gameObject
            );
        }
    }

    /// <summary>
    /// Death 애니메이션 마지막 프레임의
    /// Animation Event에서 호출할 함수
    /// </summary>
    public void OnDieAnimationEnd()
    {
        if (bossMonster == null)
            return;

        bossMonster.CompleteBossDeath();
    }
}