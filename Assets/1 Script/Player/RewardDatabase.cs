using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RewardDatabase",
    menuName = "DungeonSurvivor/Reward Database")]
public class RewardDatabase : ScriptableObject
{
    public List<RewardData> rewards;
}