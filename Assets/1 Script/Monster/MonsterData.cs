using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Game/Monster Data")]
public class MonsterData : ScriptableObject
{
    [Header("기본 정보")]
    public string monsterName;
    public PoolType poolType = PoolType.None;

    [Header("스탯")]
    public float maxHp = 10f;
    public float moveSpeed = 2f;
    public float damage = 1f;
    public float attackDistance = 1f;

    [Header("원거리 공격")]
    public float projectileSpeed = 5f;
    public float attackDelay = 1f;

    [Header("처치후 경험치")]
    public int Exp = 10;
}