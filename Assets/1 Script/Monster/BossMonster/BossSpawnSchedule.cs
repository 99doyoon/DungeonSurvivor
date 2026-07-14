using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BossSpawnSchedule",
    menuName = "Game/Boss Spawn Schedule"
)]
public class BossSpawnSchedule : ScriptableObject
{
    [Header("보스 스폰 목록")]
    public List<BossSpawnData> bossSpawnList = new();
}