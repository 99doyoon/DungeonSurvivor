[System.Serializable]
public class RewardState
{
    public RewardData rewardData;
    public int currentLevel;

    public RewardState(RewardData rewardData)
    {
        this.rewardData = rewardData;
        currentLevel = 0;
    }

    public bool CanLevelUp()
    {
        return currentLevel < rewardData.maxLevel;
    }

    public void LevelUp()
    {
        currentLevel++;
    }
}