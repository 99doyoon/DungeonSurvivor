using UnityEngine;

public class TitleBgmPlayer : MonoBehaviour
{
    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        SoundManager.Instance.PlayBgm(BGMType.Title);
    }
}