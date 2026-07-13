using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class RewardManager : MonoBehaviour
{
    [Header("보상 데이터")]
    [SerializeField] private List<RewardData> rewardDataList;

    [Header("보상 적용")]
    [SerializeField] private RewardApplier rewardApplier;

    [Header("보상 UI")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private List<RewardButton> rewardButtons;

    [SerializeField] private readonly List<RewardState> rewardStates = new();

    private void Awake()
    {
        Time.timeScale = 1f;

        if (rewardPanel != null)
        {
            rewardPanel.SetActive(false);
        }

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

    public void OpenReward()
    {
        rewardPanel.SetActive(true);

        // 보상 선택 중 게임 정지
        Time.timeScale = 0f;

        List<RewardData> selectedRewards = GetRandomRewards(3);

        for (int i = 0; i < rewardButtons.Count; i++)
        {
            if (i < selectedRewards.Count)
            {
                rewardButtons[i].gameObject.SetActive(true);
                rewardButtons[i].SetReward(
                    selectedRewards[i],
                    SelectReward);
            }
            else
            {
                rewardButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void CloseReward()
    {
        rewardPanel.SetActive(false);

        // 게임 시간 정상화
        Time.timeScale = 1f;
    }

    private List<RewardData> GetRandomRewards(int count)
    {
        List<RewardData> copyList =
            new List<RewardData>(rewardDataList);

        List<RewardData> result =
            new List<RewardData>();

        while (result.Count < count && copyList.Count > 0)
        {
            int randomIndex =
                Random.Range(0, copyList.Count);

            result.Add(copyList[randomIndex]);
            copyList.RemoveAt(randomIndex);
        }

        return result;
    }

    public void SelectReward(RewardData selectedReward)
    {
        Debug.Log($"RewardManager에서 선택: {selectedReward.rewardName}");

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
        CloseReward();

#if UNITY_EDITOR
        Debug.Log(
            $"{selectedReward.rewardName} 선택 " +
            $"현재 레벨: {state.currentLevel}/{selectedReward.maxLevel}");
#endif
    }
}