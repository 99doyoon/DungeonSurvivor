using UnityEngine;

public class MapChunk : MonoBehaviour
{
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
}