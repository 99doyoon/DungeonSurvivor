using UnityEngine;

//레벨업 보상종류 설명
public enum RewardType
{
    None,
    IncreaseDamage,
    IncreaseAttackSpeed,
    IncreaseMoveSpeed,
    IncreaseMaxHp,
    IncreaseNumberOfAttacks,
    Heal,
    AddProjectile,
    IncreaseProjectileSpeed
}

[CreateAssetMenu(
    fileName = "RewardData",
    menuName = "Game/Reward Data")]
public class RewardData : ScriptableObject
{
    [Header("기본 정보")]
    public string rewardName;

    [TextArea]
    public string description;

    public Sprite icon;

    [Header("보상 효과")]
    public RewardType rewardType;

    public float value;

    [Header("등장 조건")]
    public int maxLevel = 5;

    public int weight = 10;

    [Header("최대 강화 횟수")]
    public int maxSkillLevel = 5;
}
