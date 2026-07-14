using UnityEngine;

public class GameBgmPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip gameBgm;

    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        SoundManager.Instance.PlayBgm(gameBgm);
    }
}