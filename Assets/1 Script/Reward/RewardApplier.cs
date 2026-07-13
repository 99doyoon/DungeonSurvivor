using UnityEngine;

public class RewardApplier : MonoBehaviour
{
    [SerializeField]
    private PlayerStatus playerStatus;

    private void Awake()
    {
        if (playerStatus == null)
        {
            playerStatus = GetComponent<PlayerStatus>();
        }
    }

    public void ApplyReward(RewardData reward)
    {
        if (reward == null)
        {
            Debug.LogError("RewardData가 null입니다.");
            return;
        }

        switch (reward.rewardType)
        {
            case RewardType.None:
                break;

            case RewardType.IncreaseDamage:
                playerStatus.IncreaseDamage(reward.value);
                break;

            case RewardType.IncreaseAttackSpeed:
                playerStatus.IncreaseAttackSpeed(reward.value);
                break;

            case RewardType.IncreaseMoveSpeed:
                playerStatus.IncreaseMoveSpeed(reward.value);
                break;

            case RewardType.IncreaseMaxHp:
                playerStatus.IncreaseMaxHp(
                    Mathf.RoundToInt(reward.value));
                break;

            case RewardType.Heal:
                playerStatus.Heal(
                    Mathf.RoundToInt(reward.value));
                break;

            case RewardType.AddProjectile:
                playerStatus.AddProjectile(
                    Mathf.RoundToInt(reward.value));
                break;

            case RewardType.IncreaseProjectileSpeed:
                playerStatus.IncreaseProjectileSpeed(reward.value);
                break;

            default:
                Debug.LogWarning(
                    $"처리되지 않은 보상입니다: {reward.rewardType}");
                break;
        }
    }
}