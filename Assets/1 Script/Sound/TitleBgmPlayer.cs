using UnityEngine;

public class TitleBgmPlayer : MonoBehaviour
{
    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("SoundManager가 존재하지 않습니다.");
            return;
        }

        SoundManager.Instance.PlayBgm(BGMType.Title);
    }
}