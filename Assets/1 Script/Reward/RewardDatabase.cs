using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RewardDatabase",
    menuName = "Game/Reward Database")]
public class RewardDatabase : ScriptableObject
{
    public List<RewardData> rewards;
}