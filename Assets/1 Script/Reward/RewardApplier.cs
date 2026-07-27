using UnityEngine;

[RequireComponent(typeof(PlayerStatus))]
public class RewardApplier : MonoBehaviour
{
    [SerializeField]
    private PlayerStatus playerStatus;

    [SerializeField]
    private ProjectileEffectManager projectileEffectManager;

    private void Awake()
    {
        // RewardApplier와 같은 Player에 있는 PlayerStatus를 가져온다.
        playerStatus = GetComponent<PlayerStatus>();

        if (playerStatus == null)
        {

        }

    }

    //보상에따른 함수 호출
    public void ApplyReward(RewardData reward)
    {
        if (reward == null)
        {
            return;
        }

        GameResultManager.Instance?.AddSkillLevel(reward.rewardName);

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
                     playerStatus.IncreaseAutoHealAmount(
                        Mathf.RoundToInt(reward.value));
                break;

            case RewardType.AddProjectile:
                playerStatus.AddProjectile(
                    Mathf.RoundToInt(reward.value));
                break;

            case RewardType.IncreaseProjectileSpeed:
                playerStatus.IncreaseProjectileSpeed(reward.value);
                break;

            case RewardType.IncreaseAutoHeal:
                playerStatus.IncreaseAutoHealAmount(
                    Mathf.RoundToInt(reward.value));
                break;

            case RewardType.DecreaseAutoHealInterval:
                playerStatus.DecreaseAutoHealInterval(reward.value);
                break;

            case RewardType.ExplosionProjectile:
                projectileEffectManager.AddExplosion();
                break;

            case RewardType.BounceProjectile:
                projectileEffectManager.AddBounce();
                break;

            default:
                break;
        }
    }
}