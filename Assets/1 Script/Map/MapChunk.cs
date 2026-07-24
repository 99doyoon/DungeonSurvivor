using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapChunk : MonoBehaviour
{
    [Header("배럴 생성")]
    [SerializeField]
    private int minBarrelCount = 0;

    [SerializeField]
    private int maxBarrelCount = 2;

    [SerializeField]
    [Range(0f, 1f)]
    private float barrelSpawnChance = 0.7f;

    [SerializeField]
    private float spawnPadding = 2f;

    [SerializeField]
    private float overlapCheckRadius = 0.6f;

    [SerializeField]
    private LayerMask blockedLayer;

    private readonly List<ExplosiveBarrel>
        spawnedBarrels =
            new List<ExplosiveBarrel>();

    private float chunkSize;
    private bool isStartChunk;


    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap decorationTilemap;
    [SerializeField] private Tilemap obstacleTilemap;

    public Vector2Int ChunkPosition { get; private set; }

    public void Initialize(
    Vector2Int chunkPosition,
    float chunkSize)
    {
        ClearBarrels();

        ChunkPosition = chunkPosition;
        this.chunkSize = chunkSize;

        isStartChunk =
            chunkPosition == Vector2Int.zero;

        transform.position = new Vector3(
            chunkPosition.x * chunkSize,
            chunkPosition.y * chunkSize,
            0f
        );

        gameObject.SetActive(true);

        if (!isStartChunk)
        {
            SpawnBarrels();
        }
    }

    public void ReturnToPool()
    {
        ClearBarrels();

        ChunkPosition = Vector2Int.zero;

        gameObject.SetActive(false);
    }

    private void SpawnBarrels()
    {
        if (ObjectPool.instance == null)
        {
            return;
        }

        if (Random.value > barrelSpawnChance)
        {
            return;
        }

        int spawnCount = Random.Range(
            minBarrelCount,
            maxBarrelCount + 1
        );

        for (int i = 0; i < spawnCount; i++)
        {
            TrySpawnBarrel();
        }
    }

    private void TrySpawnBarrel()
    {
        const int maxAttempts = 10;

        for (int attempt = 0;
             attempt < maxAttempts;
             attempt++)
        {
            Vector2 spawnPosition =
                GetRandomPosition();

            bool isBlocked =
                Physics2D.OverlapCircle(
                    spawnPosition,
                    overlapCheckRadius,
                    blockedLayer
                ) != null;

            if (isBlocked)
            {
                continue;
            }

            CreateBarrel(spawnPosition);
            return;
        }
    }

    private Vector2 GetRandomPosition()
    {
        float halfSize = chunkSize * 0.5f;

        float min = -halfSize + spawnPadding;
        float max = halfSize - spawnPadding;

        Vector2 localPosition =
            new Vector2(
                Random.Range(min, max),
                Random.Range(min, max)
            );

        return (Vector2)transform.position +
               localPosition;
    }

    private void CreateBarrel(
    Vector2 spawnPosition)
    {
        ExplosiveBarrel barrel =
            ObjectPool.instance
                .GetObject<ExplosiveBarrel>(
                    PoolType.ExplosiveBarrel
                );

        if (barrel == null)
        {
            Debug.LogWarning(
                "폭발 배럴을 풀에서 가져오지 못했습니다.",
                gameObject
            );

            return;
        }

        barrel.transform.position =
            spawnPosition;

        barrel.transform.rotation =
            Quaternion.identity;

        barrel.transform.SetParent(
            transform,
            true
        );

        barrel.OnReturnedToPool +=
            HandleBarrelReturned;

        spawnedBarrels.Add(barrel);
    }

    private void HandleBarrelReturned(
    ExplosiveBarrel barrel)
    {
        if (barrel == null)
        {
            return;
        }

        barrel.OnReturnedToPool -=
            HandleBarrelReturned;

        spawnedBarrels.Remove(barrel);
    }

    private void ClearBarrels()
    {
        for (int i = spawnedBarrels.Count - 1;
             i >= 0;
             i--)
        {
            ExplosiveBarrel barrel =
                spawnedBarrels[i];

            if (barrel == null)
            {
                spawnedBarrels.RemoveAt(i);
                continue;
            }

            barrel.OnReturnedToPool -=
                HandleBarrelReturned;

            if (barrel.gameObject.activeSelf &&
                ObjectPool.instance != null)
            {
                ObjectPool.instance
                    .ReturnObject(barrel);
            }

            spawnedBarrels.RemoveAt(i);
        }
    }

    private void OnDisable()
    {
        ClearBarrels();
    }
}