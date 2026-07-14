using UnityEngine;

[CreateAssetMenu(
    fileName = "BossPatternData",
    menuName = "Game/Boss Pattern Data"
)]
public class BossPatternData : ScriptableObject
{
    [Header("이동 설정")]
    [Tooltip("플레이어를 추적할 때의 이동 속도")]
    public float moveSpeed = 2f;

    [Header("패턴 공통 설정")]
    [Tooltip("다음 특수 패턴을 사용할 수 있을 때까지의 시간")]
    public float patternCooldown = 5f;

    [Tooltip("패턴이 실행되기 전의 예고 시간")]
    public float patternWarningTime = 1f;

    [Tooltip("패턴 실행 후 보스가 쉬는 시간")]
    public float patternAfterDelay = 1f;

    [Header("공통 투사체 설정")]
    [Tooltip("보스가 사용할 투사체의 오브젝트 풀 타입")]
    public PoolType bulletPoolType = PoolType.Bullet;

    [Tooltip("투사체가 플레이어에게 입힐 피해량")]
    public float bulletDamage = 5f;

    [Header("원형 발사 설정")]
    [Tooltip("원형으로 발사할 투사체 개수")]
    [Min(1)]
    public int circleBulletCount = 12;

    [Tooltip("원형 투사체 이동 속도")]
    public float circleBulletSpeed = 4f;

    [Header("부채꼴 발사 설정")]
    [Tooltip("부채꼴로 발사할 투사체 개수")]
    [Min(1)]
    public int fanBulletCount = 5;

    [Tooltip("부채꼴 투사체 이동 속도")]
    public float fanBulletSpeed = 5f;

    [Tooltip("각 투사체 사이의 각도")]
    public float fanSpreadAngle = 15f;

    [Header("돌진 패턴 설정")]
    [Tooltip("돌진할 때의 이동 속도")]
    public float dashSpeed = 10f;

    [Tooltip("돌진 상태를 유지하는 시간")]
    public float dashDuration = 0.5f;
}