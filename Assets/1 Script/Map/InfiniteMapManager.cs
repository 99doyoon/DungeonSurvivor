using System.Collections.Generic;
using UnityEngine;

public class InfiniteMapManager : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Chunk")]
    [SerializeField] private ChunkPool chunkPool;
    [SerializeField] private MapChunk startChunkPrefab;
    [SerializeField] private float chunkSize = 20f;

    [Header("View")]
    [SerializeField] private int viewDistance = 1;

    [Header("Seed")]
    [SerializeField] private int mapSeed;

    private readonly Dictionary<Vector2Int, MapChunk> activeChunks
        = new Dictionary<Vector2Int, MapChunk>();

    private Vector2Int previousPlayerChunk;

    private void Awake()
    {
        mapSeed = Random.Range(int.MinValue, int.MaxValue);
    }

    private void Start()
    {
        previousPlayerChunk = GetPlayerChunkPosition();
        UpdateChunks();
    }

    private void Update()
    {
        Vector2Int currentPlayerChunk = GetPlayerChunkPosition();

        if (currentPlayerChunk == previousPlayerChunk)
        {
            return;
        }

        previousPlayerChunk = currentPlayerChunk;

        UpdateChunks();
    }

    private Vector2Int GetPlayerChunkPosition()
    {
        return new Vector2Int(
            Mathf.FloorToInt(player.position.x / chunkSize),
            Mathf.FloorToInt(player.position.y / chunkSize)
        );
    }

    private void UpdateChunks()
    {
        Vector2Int playerChunk = GetPlayerChunkPosition();

        HashSet<Vector2Int> requiredChunks =
            new HashSet<Vector2Int>();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkPosition =
                    playerChunk + new Vector2Int(x, y);

                requiredChunks.Add(chunkPosition);

                if (!activeChunks.ContainsKey(chunkPosition))
                {
                    CreateChunk(chunkPosition);
                }
            }
        }

        ReturnFarChunks(requiredChunks);
    }

    private void CreateChunk(Vector2Int chunkPosition)
    {
        MapChunk selectedPrefab =
            SelectChunkPrefab(chunkPosition);

        if (selectedPrefab == null)
        {
            Debug.LogError(
                $"{chunkPosition}에 사용할 청크가 없습니다.");

            return;
        }

        MapChunk chunk = chunkPool.GetChunk(selectedPrefab);

        if (chunk == null)
        {
            return;
        }

        chunk.transform.SetParent(transform);
        chunk.Initialize(chunkPosition, chunkSize);

        activeChunks.Add(chunkPosition, chunk);
    }

    private void ReturnFarChunks(
        HashSet<Vector2Int> requiredChunks)
    {
        List<Vector2Int> returnPositions =
            new List<Vector2Int>();

        foreach (KeyValuePair<Vector2Int, MapChunk> pair
                 in activeChunks)
        {
            if (!requiredChunks.Contains(pair.Key))
            {
                returnPositions.Add(pair.Key);
            }
        }

        foreach (Vector2Int position in returnPositions)
        {
            MapChunk chunk = activeChunks[position];

            activeChunks.Remove(position);
            chunkPool.ReturnChunk(chunk);
        }
    }

    private MapChunk SelectChunkPrefab(
        Vector2Int chunkPosition)
    {
        // 시작 지점은 안전한 청크로 고정
        if (chunkPosition == Vector2Int.zero &&
            startChunkPrefab != null)
        {
            return startChunkPrefab;
        }

        IReadOnlyList<ChunkPoolData> chunkList =
            chunkPool.ChunkPoolList;

        if (chunkList == null || chunkList.Count == 0)
        {
            return null;
        }

        int totalWeight = 0;

        foreach (ChunkPoolData data in chunkList)
        {
            if (data.prefab == null)
            {
                continue;
            }

            totalWeight += data.weight;
        }

        if (totalWeight <= 0)
        {
            return null;
        }

        int seed =
            chunkPosition.x * 73856093
            ^ chunkPosition.y * 19349663
            ^ mapSeed;

        System.Random random = new System.Random(seed);

        int randomValue = random.Next(0, totalWeight);

        foreach (ChunkPoolData data in chunkList)
        {
            if (data.prefab == null)
            {
                continue;
            }

            if (randomValue < data.weight)
            {
                return data.prefab;
            }

            randomValue -= data.weight;
        }

        return null;
    }
}