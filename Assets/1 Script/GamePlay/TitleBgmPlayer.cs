using UnityEngine;

public class TitleBgmPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip titleBgm;

    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        SoundManager.Instance.PlayBgm(titleBgm);
    }
}