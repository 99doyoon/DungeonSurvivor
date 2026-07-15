using UnityEngine;

[System.Serializable]
public class ChunkPoolData
{
    public MapChunk prefab;

    [Min(1)]
    public int weight = 1;

    [Min(1)]
    public int initialCount = 3;
}