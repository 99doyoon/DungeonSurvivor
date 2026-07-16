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

    [Header("Map Region")]
    [Tooltip("값이 작을수록 같은 종류의 청크가 더 넓게 이어집니다.")]
    [SerializeField, Range(0.01f, 1f)]
    private float regionScale = 0.15f;

    // PerlinNoise의 시작 위치
    private Vector2 noiseOffset;

    private readonly Dictionary<Vector2Int, MapChunk> activeChunks
        = new Dictionary<Vector2Int, MapChunk>();

    private Vector2Int previousPlayerChunk;

    private void Awake()
    {
        // 실행할 때마다 새로운 맵을 만들기 위한 시드
        mapSeed = Random.Range(int.MinValue, int.MaxValue);

        // Unity의 Random 상태에 영향을 주지 않도록
        // System.Random을 사용해 노이즈 시작 위치를 만든다.
        System.Random seedRandom =
            new System.Random(mapSeed);

        noiseOffset = new Vector2(
            seedRandom.Next(-10000, 10000),
            seedRandom.Next(-10000, 10000)
        );
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (player == null)
        {
            Debug.LogError(
                "InfiniteMapManager에 Player가 연결되지 않았습니다.",
                gameObject
            );

            enabled = false;
            return;
        }

        if (chunkPool == null)
        {
            Debug.LogError(
                "InfiniteMapManager에 ChunkPool이 연결되지 않았습니다.",
                gameObject
            );

            enabled = false;
            return;
        }

        if (chunkSize <= 0f)
        {
            Debug.LogError(
                "Chunk Size는 0보다 커야 합니다.",
                gameObject
            );

            enabled = false;
            return;
        }
#endif

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
                $"{chunkPosition}에 사용할 청크가 없습니다."
            );

            return;
        }

        MapChunk chunk =
            chunkPool.GetChunk(selectedPrefab);

        if (chunk == null)
            return;

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

    /// <summary>
    /// 청크의 월드 좌표를 기반으로 사용할 청크 프리팹을 선택한다.
    ///
    /// 완전히 독립적인 랜덤값을 사용하지 않고 PerlinNoise를 사용하기 때문에
    /// 가까이 있는 청크들이 비슷한 값을 받아 같은 지형이 모여서 생성된다.
    /// </summary>
    private MapChunk SelectChunkPrefab(
        Vector2Int chunkPosition)
    {
        // 게임 시작 위치는 안전한 청크로 고정한다.
        if (chunkPosition == Vector2Int.zero &&
            startChunkPrefab != null)
        {
            return startChunkPrefab;
        }

        IReadOnlyList<ChunkPoolData> chunkList =
            chunkPool.ChunkPoolList;

        if (chunkList == null || chunkList.Count == 0)
        {
            Debug.LogError(
                "ChunkPool에 사용할 청크 데이터가 없습니다.",
                gameObject
            );

            return null;
        }

        // 사용할 수 있는 청크들의 전체 가중치를 계산한다.
        int totalWeight = 0;

        foreach (ChunkPoolData data in chunkList)
        {
            if (data == null || data.prefab == null)
                continue;

            // 0 이하의 가중치는 선택 대상에서 제외한다.
            if (data.weight <= 0)
                continue;

            totalWeight += data.weight;
        }

        if (totalWeight <= 0)
        {
            Debug.LogError(
                "청크 가중치의 합이 0 이하입니다.",
                gameObject
            );

            return null;
        }

        /*
         * 청크 좌표를 PerlinNoise 좌표로 변환한다.
         *
         * 가까운 청크 좌표는 가까운 노이즈 값을 가지므로
         * 같은 종류의 청크가 지역 단위로 뭉치게 된다.
         */
        float noiseX =
            chunkPosition.x * regionScale + noiseOffset.x;

        float noiseY =
            chunkPosition.y * regionScale + noiseOffset.y;

        // 반환 범위: 0 ~ 1
        float noiseValue =
            Mathf.PerlinNoise(noiseX, noiseY);

        /*
         * 0~1 범위의 노이즈 값을
         * 0~전체 가중치 범위로 변환한다.
         */
        float weightedValue =
            noiseValue * totalWeight;

        foreach (ChunkPoolData data in chunkList)
        {
            if (data == null || data.prefab == null)
                continue;

            if (data.weight <= 0)
                continue;

            if (weightedValue < data.weight)
            {
                return data.prefab;
            }

            weightedValue -= data.weight;
        }

        // 부동소수점 오차 때문에 선택되지 않았을 경우를 대비한 안전장치
        for (int i = chunkList.Count - 1; i >= 0; i--)
        {
            ChunkPoolData data = chunkList[i];

            if (data != null &&
                data.prefab != null &&
                data.weight > 0)
            {
                return data.prefab;
            }
        }

        return null;
    }

    //Debugging chunk size
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(
            transform.position +
            new Vector3(chunkSize * 0.5f, chunkSize * 0.5f, 0f),
            new Vector3(chunkSize, chunkSize, 0f)
        );
    }
}