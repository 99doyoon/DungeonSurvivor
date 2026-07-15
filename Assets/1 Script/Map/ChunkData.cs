using UnityEngine;

[System.Serializable]
public class ChunkData
{
    public MapChunk prefab;

    [Min(1)]
    public int weight = 1;
}