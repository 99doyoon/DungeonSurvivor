using UnityEngine;

public class GameBgmPlayer : MonoBehaviour
{
    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        SoundManager.Instance.PlayBgm(BGMType.Game);
    }
}