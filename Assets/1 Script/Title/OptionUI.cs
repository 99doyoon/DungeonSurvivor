using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        if (SoundManager.Instance == null)
        {
            return;
        }

        bgmSlider.value = SoundManager.Instance.BgmVolume;
        sfxSlider.value = SoundManager.Instance.SfxVolume;

        bgmSlider.onValueChanged.AddListener(ChangeBgmVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSfxVolume);
    }

    private void ChangeBgmVolume(float value)
    {
        SoundManager.Instance.SetBgmVolume(value);
    }

    private void ChangeSfxVolume(float value)
    {
        SoundManager.Instance.SetSfxVolume(value);
    }

    private void OnDestroy()
    {
        bgmSlider.onValueChanged.RemoveListener(ChangeBgmVolume);
        sfxSlider.onValueChanged.RemoveListener(ChangeSfxVolume);
    }
}