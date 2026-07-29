using UnityEngine;

[CreateAssetMenu(
    fileName = "EliteMonsterData",
    menuName = "Game/Elite Monster Data"
)]
public class EliteMonsterData : ScriptableObject
{
    [Header("등장 확률")]
    [Range(0f, 1f)]
    public float spawnChance = 0.08f;

    [Header("능력치 배율")]
    [Min(1f)]
    public float hpMultiplier = 2f;

    [Min(0f)]
    public float damageMultiplier = 1.5f;

    [Min(0f)]
    public float moveSpeedMultiplier = 1.1f;

    [Min(0f)]
    public float expMultiplier = 3f;

    [Header("외형")]
    [Min(0.1f)]
    public float scaleMultiplier = 1.25f;

    public Color eliteColor =
        new Color(1f, 0.65f, 0.2f, 1f);
}