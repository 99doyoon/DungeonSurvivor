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