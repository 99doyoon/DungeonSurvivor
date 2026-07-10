//using System.Collections.Generic;
//using UnityEngine;

//public class MonsterSpawnManager : MonoBehaviour
//{
//    [Header("Player")]
//    [SerializeField] private Transform player;

//    [Header("Spawn Range")]
//    [SerializeField] private float minSpawnDistance = 8f;   // 이 거리 안에는 스폰 금지
//    [SerializeField] private float maxSpawnDistance = 12f;  // 이 거리 안쪽까지 랜덤 스폰

//    [Header("Spawn Time")]
//    [SerializeField] private float spawnInterval = 1.5f;    // 몇 초마다 한 번 스폰할지
//    private float elapsedTime;                              // 게임 시작 후 누적 시간
//    private float spawnTimer;                              // 다음 스폰까지의 타이머

//    [Header("Monster Spawn Table")]
//    [SerializeField] private List<MonsterSpawnData> monsterSpawnTable = new();

//    private void Start()
//    {
//        // 인스펙터에서 player를 안 넣었을 경우 자동 탐색
//        if (player == null)
//        {
//            GameObject playerObj = GameObject.FindWithTag("Player");
//            if (playerObj != null)
//                player = playerObj.transform;
//        }
//    }

//    private void Update()
//    {
//        if (player == null)
//            return;

//        elapsedTime += Time.deltaTime;
//        spawnTimer += Time.deltaTime;

//        if (spawnTimer >= spawnInterval)
//        {
//            spawnTimer = 0f;
//            SpawnMonster();
//        }
//    }

//    private void SpawnMonster()
//    {
//        // 현재 시간에 등장 가능한 몬스터 중 하나를 가중치 랜덤으로 선택
//        PoolType monsterType = GetRandomMonsterByTime(elapsedTime);

//        if (monsterType == PoolType.None)
//            return;

//        Vector2 spawnPos = GetRandomSpawnPosition();

//        GameObject monster = ObjectPool.instance.GetObject(monsterType);
//        if (monster == null)
//            return;

//        monster.transform.position = spawnPos;
//        monster.transform.rotation = Quaternion.identity;
//    }

//    /// <summary>
//    /// 현재 시간에 등장 가능한 몬스터들만 추린 뒤,
//    /// weight를 기준으로 랜덤 1종 선택해서 PoolType 반환
//    /// </summary>
//    private PoolType GetRandomMonsterByTime(float currentTime)
//    {
//        List<MonsterSpawnData> availableMonsters = new List<MonsterSpawnData>();

//        // 1. 현재 시간에 등장 가능한 몬스터만 수집
//        foreach (var monsterData in monsterSpawnTable)
//        {
//            if (monsterData.monsterType == PoolType.None)
//                continue;

//            if (currentTime >= monsterData.startTime && currentTime < monsterData.endTime)
//            {
//                availableMonsters.Add(monsterData);
//            }
//        }

//        // 등장 가능한 몬스터가 없으면 None
//        if (availableMonsters.Count == 0)
//            return PoolType.None;

//        // 2. 총 가중치 계산
//        int totalWeight = 0;
//        foreach (var monsterData in availableMonsters)
//        {
//            if (monsterData.weight > 0)
//                totalWeight += monsterData.weight;
//        }

//        if (totalWeight <= 0)
//            return PoolType.None;

//        // 3. 0 ~ totalWeight-1 사이 랜덤값 선택
//        int rand = Random.Range(0, totalWeight);
//        int currentWeight = 0;

//        // 4. 누적 가중치로 랜덤 몬스터 선택
//        foreach (var monsterData in availableMonsters)
//        {
//            if (monsterData.weight <= 0)
//                continue;

//            currentWeight += monsterData.weight;

//            if (rand < currentWeight)
//                return monsterData.monsterType;
//        }

//        return PoolType.None;
//    }

//    /// <summary>
//    /// 플레이어 주변 minSpawnDistance ~ maxSpawnDistance 사이의 랜덤 위치 반환
//    /// </summary>
//    private Vector2 GetRandomSpawnPosition()
//    {
//        // 랜덤 방향
//        Vector2 dir = Random.insideUnitCircle.normalized;

//        // 최소 거리 ~ 최대 거리 사이의 랜덤 거리
//        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

//        // 플레이어 위치 기준으로 스폰 위치 계산
//        Vector2 playerPos = player.position;
//        Vector2 spawnPos = playerPos + dir * distance;

//        return spawnPos;
//    }
//}

//gpt
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("UI")]
    [SerializeField] private Canvas uiCanvas;

    [Header("Spawn Range")]
    [SerializeField] private float minSpawnDistance = 8f;
    [SerializeField] private float maxSpawnDistance = 12f;

    [Header("Spawn Time")]
    [SerializeField] private float spawnInterval = 1.5f;

    [Header("Monster Spawn Table")]
    [SerializeField] private List<MonsterSpawnData> monsterSpawnTable = new();

    private float elapsedTime;
    private float spawnTimer;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        elapsedTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnMonster();
        }
    }

    private void SpawnMonster()
    {
        PoolType monsterType = GetRandomMonsterByTime(elapsedTime);
        if (monsterType == PoolType.None)
            return;

        Vector2 spawnPos = GetRandomSpawnPosition();

        GameObject monster = ObjectPool.instance.GetObject(monsterType);
        if (monster == null)
            return;

        monster.transform.position = spawnPos;
        monster.transform.rotation = Quaternion.identity;

        EnemyBase enemy = monster.GetComponent<EnemyBase>();
        if (enemy == null)
        {
            Debug.LogError($"{monster.name}에 EnemyBase가 없습니다.");
            return;
        }

        CreateHpBar(enemy);
    }

    private void CreateHpBar(EnemyBase enemy)
    {
        if (uiCanvas == null)
        {
            Debug.LogError("MonsterSpawnManager에 uiCanvas가 연결되지 않았습니다.");
            return;
        }

        GameObject hpBarObj = ObjectPool.instance.GetObject(PoolType.EnemyHpBar);
        if (hpBarObj == null)
            return;

        hpBarObj.transform.SetParent(uiCanvas.transform, false);
        hpBarObj.transform.localScale = Vector3.one;

        EnemyHpBar hpBar = hpBarObj.GetComponent<EnemyHpBar>();
        if (hpBar == null)
        {
            Debug.LogError("EnemyHpBar 프리팹에 EnemyHpBar 컴포넌트가 없습니다.");
            return;
        }

        hpBar.SetTarget(enemy);
    }

    private PoolType GetRandomMonsterByTime(float currentTime)
    {
        List<MonsterSpawnData> availableMonsters = new();

        foreach (MonsterSpawnData data in monsterSpawnTable)
        {
            if (data.monsterType == PoolType.None)
                continue;

            if (currentTime >= data.startTime && currentTime < data.endTime)
                availableMonsters.Add(data);
        }

        if (availableMonsters.Count == 0)
            return PoolType.None;

        int totalWeight = 0;
        foreach (MonsterSpawnData data in availableMonsters)
        {
            if (data.weight > 0)
                totalWeight += data.weight;
        }

        if (totalWeight <= 0)
            return PoolType.None;

        int rand = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (MonsterSpawnData data in availableMonsters)
        {
            if (data.weight <= 0)
                continue;

            currentWeight += data.weight;

            if (rand < currentWeight)
                return data.monsterType;
        }

        return PoolType.None;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
        return (Vector2)player.position + dir * distance;
    }
}