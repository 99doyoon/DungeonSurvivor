using UnityEngine;
using UnityEngine.InputSystem;

public class RewardTest : MonoBehaviour
{
    [SerializeField] private RewardApplier rewardApplier;
    [SerializeField] private RewardData rewardData;

    private void Awake()
    {
        if (rewardApplier == null)
        {
            rewardApplier = GetComponent<RewardApplier>();
        }
    }

    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            rewardApplier.ApplyReward(rewardData);
        }
    }
}