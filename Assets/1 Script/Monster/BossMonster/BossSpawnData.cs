using System;
using UnityEngine;

[Serializable]
public class BossSpawnData
{
    [Header("보스 프리팹")]
    public GameObject bossPrefab;

    [Header("등장 시간")]
    [Min(0f)]
    public float spawnTime = 60f;

    [Header("스폰 거리")]
    [Min(0f)]
    public float minSpawnDistance = 10f;

    [Min(0f)]
    public float maxSpawnDistance = 14f;
}