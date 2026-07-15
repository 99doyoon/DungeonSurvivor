using System.Collections.Generic;
using UnityEngine;

public class InfiniteMapManager : MonoBehaviour
{
    [SerializeField] private int mapSeed;

    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Chunk")]
    [SerializeField] private List<ChunkData> chunkList;
    [SerializeField] private float chunkSize = 20f;

    [Header("View Range")]
    [SerializeField] private int viewDistance = 1;

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
        int chunkX = Mathf.FloorToInt(player.position.x / chunkSize);
        int chunkY = Mathf.FloorToInt(player.position.y / chunkSize);

        return new Vector2Int(chunkX, chunkY);
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

        RemoveFarChunks(requiredChunks);
    }

    private void CreateChunk(Vector2Int chunkPosition)
    {
        MapChunk selectedPrefab = GetChunkPrefab(chunkPosition);

        if (selectedPrefab == null)
        {
            Debug.LogError("생성할 청크 프리팹을 찾지 못했습니다.");
            return;
        }

        MapChunk chunk = Instantiate(
            selectedPrefab,
            transform
        );

        chunk.Initialize(chunkPosition, chunkSize);

        activeChunks.Add(chunkPosition, chunk);
    }

    private void RemoveFarChunks(
        HashSet<Vector2Int> requiredChunks)
    {
        List<Vector2Int> removeList =
            new List<Vector2Int>();

        foreach (var chunk in activeChunks)
        {
            if (!requiredChunks.Contains(chunk.Key))
            {
                removeList.Add(chunk.Key);
            }
        }

        foreach (Vector2Int position in removeList)
        {
            Destroy(activeChunks[position].gameObject);
            activeChunks.Remove(position);
        }
    }

    private MapChunk GetChunkPrefab(Vector2Int chunkPosition)
    {
        if (chunkList == null || chunkList.Count == 0)
        {
            return null;
        }

        int totalWeight = 0;

        foreach (ChunkData data in chunkList)
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

        int seed = chunkPosition.x * 73856093
                 ^ chunkPosition.y * 19349663
                 ^ mapSeed;

        System.Random random = new System.Random(seed);

        int randomValue = random.Next(0, totalWeight);

        foreach (ChunkData data in chunkList)
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

        return chunkList[0].prefab;
    }
}