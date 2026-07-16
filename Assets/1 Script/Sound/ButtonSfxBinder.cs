using UnityEngine;
using UnityEngine.UI;

public class ButtonSfxBinder : MonoBehaviour
{
    private void Start()
    {
        // 현재 씬에 활성화된 모든 Button을 찾는다.
        Button[] buttons = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button button in buttons)
        {
            // 중복 등록 방지
            button.onClick.RemoveListener(PlayButtonSound);
            button.onClick.AddListener(PlayButtonSound);
        }
    }

    private void PlayButtonSound()
    {
        SoundManager.Instance?.PlaySfx(SFXType.Button);
    }
}