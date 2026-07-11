[System.Serializable]
public class RewardState
{
    public RewardData data;
    public int currentLevel;

    public RewardState(RewardData data)
    {
        this.data = data;
        currentLevel = 0;
    }

    public bool CanLevelUp()
    {
        return currentLevel < data.maxLevel;
    }
}