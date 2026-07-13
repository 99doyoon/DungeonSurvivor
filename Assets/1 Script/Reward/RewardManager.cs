using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private RewardApplier rewardApplier;
    [SerializeField] private List<RewardData> rewardDataList;

    private readonly List<RewardState> rewardStates = new();

    private void Awake()
    {
        CreateRewardStates();
    }

    private void CreateRewardStates()
    {
        rewardStates.Clear();

        foreach (RewardData rewardData in rewardDataList)
        {
            RewardState state = new RewardState(rewardData);
            rewardStates.Add(state);
        }
    }

    public void SelectReward(RewardData selectedReward)
    {
        RewardState state = rewardStates.Find(
            rewardState => rewardState.rewardData == selectedReward);

        if (state == null)
        {
            Debug.LogError("선택한 보상 데이터를 찾을 수 없습니다.");
            return;
        }

        if (!state.CanLevelUp())
        {
            Debug.Log($"{selectedReward.rewardName}은 최대 레벨입니다.");
            return;
        }

        rewardApplier.ApplyReward(selectedReward);
        state.LevelUp();

#if UNITY_EDITOR
        Debug.Log(
            $"{selectedReward.rewardName} 선택 " +
            $"현재 레벨: {state.currentLevel}/{selectedReward.maxLevel}");
#endif
    }
}