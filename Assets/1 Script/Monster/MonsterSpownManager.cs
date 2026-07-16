using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("UI")]
    [SerializeField] private Canvas uiCanvas;

    [Header("일반 몬스터 스폰 범위")]
    [SerializeField] private float minSpawnDistance = 8f;
    [SerializeField] private float maxSpawnDistance = 12f;

    [Header("일반 몬스터 스폰 시간")]
    [SerializeField] private float spawnInterval = 1.5f;

    [Header("일반 몬스터 스폰 테이블")]
    [SerializeField]
    private List<MonsterSpawnData> monsterSpawnTable = new();

    [Header("보스 스폰 데이터")]
    [SerializeField]
    private BossSpawnSchedule bossSpawnSchedule;

    // 게임 시작 후 흐른 전체 시간
    private float elapsedTime;

    // 일반 몬스터 스폰 타이머
    private float spawnTimer;

    // 이미 소환된 보스 데이터의 인덱스를 저장한다.
    // ScriptableObject 자체에는 실행 여부를 기록하지 않는다.
    private readonly HashSet<int> spawnedBossIndexes = new();

    private void Start()
    {
        FindPlayer();

        // 게임을 다시 시작할 때 보스 소환 기록 초기화
        spawnedBossIndexes.Clear();
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        elapsedTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        // 일반 몬스터 스폰 처리
        UpdateNormalMonsterSpawn();

        // 보스 스폰 처리
        UpdateBossSpawn();
    }

    /// <summary>
    /// Player가 Inspector에 연결되지 않았다면
    /// Player 태그를 이용해서 찾는다.
    /// </summary>
    private void FindPlayer()
    {
        if (player != null)
            return;

        GameObject playerObj =
            GameObject.FindWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    /// <summary>
    /// 일정 시간마다 일반 몬스터를 소환한다.
    /// </summary>
    private void UpdateNormalMonsterSpawn()
    {
        if (spawnTimer < spawnInterval)
            return;

        spawnTimer = 0f;

        SpawnMonster();
    }

    /// <summary>
    /// 일반 몬스터를 소환한다.
    /// </summary>
    private void SpawnMonster()
    {
        PoolType monsterType =
            GetRandomMonsterByTime(elapsedTime);

        if (monsterType == PoolType.None)
            return;

        Vector2 spawnPos =
            GetRandomSpawnPosition(
                minSpawnDistance,
                maxSpawnDistance
            );

        GameObject monster =
            ObjectPool.instance.GetObject(monsterType);

        if (monster == null)
            return;

        monster.transform.position = spawnPos;
        monster.transform.rotation = Quaternion.identity;

        EnemyBase enemy =
            monster.GetComponent<EnemyBase>();

        if (enemy == null)
        {
            Debug.LogError(
                $"{monster.name}에 EnemyBase가 없습니다.",
                monster
            );

            ReturnPoolObject(monster);
            return;
        }

        CreateHpBar(enemy);
    }

    private bool SpawnBoss(BossSpawnData bossData)
    {
        if (bossData == null)
            return false;

        if (bossData.bossPrefab == null)
        {
            Debug.LogError(
                "BossSpawnData에 Boss Prefab이 연결되지 않았습니다."
            );

            return false;
        }

        EnemyBase prefabEnemy =
            bossData.bossPrefab.GetComponent<EnemyBase>();

        if (prefabEnemy == null)
        {
            Debug.LogError(
                $"{bossData.bossPrefab.name} 프리팹에 EnemyBase가 없습니다.",
                bossData.bossPrefab
            );

            return false;
        }

        PoolType bossPoolType = prefabEnemy.PoolType;

        if (bossPoolType == PoolType.None)
        {
            Debug.LogError(
                $"{bossData.bossPrefab.name}의 PoolType이 None입니다.",
                bossData.bossPrefab
            );

            return false;
        }

        float minDistance = bossData.minSpawnDistance;
        float maxDistance = bossData.maxSpawnDistance;

        if (maxDistance < minDistance)
        {
            maxDistance = minDistance;
        }

        Vector2 spawnPosition =
            GetRandomSpawnPosition(
                minDistance,
                maxDistance
            );

        GameObject boss =
            ObjectPool.instance.GetObject(
                bossPoolType
            );

        if (boss == null)
        {
            Debug.LogWarning(
                $"{bossPoolType} 보스를 풀에서 가져오지 못했습니다."
            );

            return false;
        }

        boss.transform.position = spawnPosition;
        boss.transform.rotation = Quaternion.identity;

        EnemyBase spawnedBossEnemy =
            boss.GetComponent<EnemyBase>();

        if (spawnedBossEnemy == null)
        {
            Debug.LogError(
                $"{boss.name}에 EnemyBase가 없습니다.",
                boss
            );

            ReturnPoolObject(boss);
            return false;
        }

        CreateHpBar(spawnedBossEnemy);

        Debug.Log(
            $"{elapsedTime:F1}초에 " +
            $"{bossData.bossPrefab.name} 보스가 등장했습니다."
        );

        // 보스 음악으로 변경
        SoundManager.Instance.PlayBgm(BGMType.Boss);

        return true;
    }

    /// <summary>
    /// BossSpawnSchedule의 시간 정보를 확인해서
    /// 아직 소환되지 않은 보스를 소환한다.
    /// </summary>
    private void UpdateBossSpawn()
    {
        if (bossSpawnSchedule == null)
            return;

        List<BossSpawnData> bossList =
            bossSpawnSchedule.bossSpawnList;

        if (bossList == null)
            return;

        for (int i = 0; i < bossList.Count; i++)
        {
            BossSpawnData bossData = bossList[i];

            if (bossData == null)
                continue;

            if (spawnedBossIndexes.Contains(i))
                continue;

            if (elapsedTime < bossData.spawnTime)
                continue;

            // 보스 소환에 성공한 경우에만
            // 해당 인덱스를 소환 완료 목록에 추가한다.
            if (SpawnBoss(bossData))
            {
                spawnedBossIndexes.Add(i);
            }
        }
    }

    /// <summary>
    /// 몬스터 또는 보스의 체력 바를 생성한다.
    /// </summary>
    private void CreateHpBar(EnemyBase enemy)
    {
        if (uiCanvas == null)
        {
            Debug.LogError(
                "MonsterSpawnManager에 uiCanvas가 연결되지 않았습니다."
            );

            return;
        }

        GameObject hpBarObj =
            ObjectPool.instance.GetObject(
                PoolType.EnemyHpBar
            );

        if (hpBarObj == null)
            return;

        // 체력 바를 UI Canvas의 자식으로 설정
        hpBarObj.transform.SetParent(
            uiCanvas.transform,
            false
        );

        hpBarObj.transform.localScale =
            Vector3.one;

        EnemyHpBar hpBar =
            hpBarObj.GetComponent<EnemyHpBar>();

        if (hpBar == null)
        {
            Debug.LogError(
                "EnemyHpBar 프리팹에 EnemyHpBar 컴포넌트가 없습니다.",
                hpBarObj
            );

            ReturnPoolObject(hpBarObj);
            return;
        }

        hpBar.SetTarget(enemy);
    }

    /// <summary>
    /// 현재 시간에 등장할 수 있는 몬스터 중 하나를
    /// 가중치 기반으로 선택한다.
    /// </summary>
    private PoolType GetRandomMonsterByTime(
        float currentTime
    )
    {
        List<MonsterSpawnData> availableMonsters =
            new();

        foreach (MonsterSpawnData data
                 in monsterSpawnTable)
        {
            if (data.monsterType == PoolType.None)
                continue;

            if (currentTime >= data.startTime &&
                currentTime < data.endTime)
            {
                availableMonsters.Add(data);
            }
        }

        if (availableMonsters.Count == 0)
            return PoolType.None;

        int totalWeight = 0;

        foreach (MonsterSpawnData data
                 in availableMonsters)
        {
            if (data.weight > 0)
            {
                totalWeight += data.weight;
            }
        }

        if (totalWeight <= 0)
            return PoolType.None;

        int rand =
            Random.Range(0, totalWeight);

        int currentWeight = 0;

        foreach (MonsterSpawnData data
                 in availableMonsters)
        {
            if (data.weight <= 0)
                continue;

            currentWeight += data.weight;

            if (rand < currentWeight)
            {
                return data.monsterType;
            }
        }

        return PoolType.None;
    }

    /// <summary>
    /// 플레이어 주변의 지정된 거리 사이에서
    /// 무작위 스폰 위치를 구한다.
    /// </summary>
    private Vector2 GetRandomSpawnPosition(
        float minDistance,
        float maxDistance
    )
    {
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        // insideUnitCircle이 정확히 0이 나오는 극단적인 경우 방어
        if (direction == Vector2.zero)
        {
            direction = Vector2.right;
        }

        float distance =
            Random.Range(minDistance, maxDistance);

        return (Vector2)player.position
               + direction * distance;
    }

    /// <summary>
    /// GameObject에서 IPoolable을 찾아 풀에 반환한다.
    /// </summary>
    private void ReturnPoolObject(GameObject target)
    {
        if (target == null)
            return;

        IPoolable poolable =
            target.GetComponent<IPoolable>();

        if (poolable != null)
        {
            ObjectPool.instance.ReturnObject(poolable);
        }
        else
        {
            target.SetActive(false);
        }
    }
}