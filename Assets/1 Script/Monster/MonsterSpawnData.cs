using UnityEngine;

// 몬스터 1종의 등장 시간/가중치 데이터
[System.Serializable]
public class MonsterSpawnData
{
    [Header("어떤 몬스터를 스폰할지")]
    public PoolType monsterType = PoolType.None;

    [Header("등장 시간")]
    public float startTime = 0f;      // 이 시간부터 등장 가능
    public float endTime = 9999f;     // 이 시간까지만 등장 가능

    [Header("확률 가중치")]
    public int weight = 1;            // 숫자가 클수록 더 자주 등장
}