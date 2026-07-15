using System.Collections.Generic;
using UnityEngine;

public class ChunkPool : MonoBehaviour
{
    [SerializeField] private List<ChunkPoolData> chunkPoolList;

    private readonly Dictionary<MapChunk, Queue<MapChunk>> pools
        = new Dictionary<MapChunk, Queue<MapChunk>>();

    private readonly Dictionary<MapChunk, MapChunk> prefabLookup
        = new Dictionary<MapChunk, MapChunk>();

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        foreach (ChunkPoolData data in chunkPoolList)
        {
            if (data.prefab == null)
            {
                continue;
            }

            if (!pools.ContainsKey(data.prefab))
            {
                pools.Add(data.prefab, new Queue<MapChunk>());
            }

            for (int i = 0; i < data.initialCount; i++)
            {
                MapChunk chunk = CreateChunk(data.prefab);
                pools[data.prefab].Enqueue(chunk);
            }
        }
    }

    private MapChunk CreateChunk(MapChunk prefab)
    {
        MapChunk chunk = Instantiate(prefab, transform);

        chunk.gameObject.SetActive(false);

        prefabLookup[chunk] = prefab;

        return chunk;
    }

    public MapChunk GetChunk(MapChunk prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("가져올 청크 프리팹이 null입니다.");
            return null;
        }

        if (!pools.TryGetValue(prefab, out Queue<MapChunk> pool))
        {
            pool = new Queue<MapChunk>();
            pools.Add(prefab, pool);
        }

        MapChunk chunk = null;

        while (pool.Count > 0 && chunk == null)
        {
            chunk = pool.Dequeue();
        }

        if (chunk == null)
        {
            chunk = CreateChunk(prefab);
        }

        return chunk;
    }

    public void ReturnChunk(MapChunk chunk)
    {
        if (chunk == null)
        {
            return;
        }

        if (!prefabLookup.TryGetValue(chunk, out MapChunk prefab))
        {
            Debug.LogWarning(
                $"{chunk.name}의 원본 프리팹 정보를 찾지 못했습니다.");

            chunk.gameObject.SetActive(false);
            return;
        }

        chunk.ReturnToPool();
        chunk.transform.SetParent(transform);

        pools[prefab].Enqueue(chunk);
    }

    public IReadOnlyList<ChunkPoolData> ChunkPoolList =>
        chunkPoolList;
}