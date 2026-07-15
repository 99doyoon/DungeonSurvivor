using UnityEngine;
using UnityEngine.Tilemaps;

public class MapChunk : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap decorationTilemap;
    [SerializeField] private Tilemap obstacleTilemap;

    public Vector2Int ChunkPosition { get; private set; }

    public void Initialize(Vector2Int chunkPosition, float chunkSize)
    {
        ChunkPosition = chunkPosition;

        transform.position = new Vector3(
            chunkPosition.x * chunkSize,
            chunkPosition.y * chunkSize,
            0f
        );

        gameObject.SetActive(true);
    }

    public void ReturnToPool()
    {
        ChunkPosition = Vector2Int.zero;
        gameObject.SetActive(false);
    }
}