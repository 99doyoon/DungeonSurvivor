using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Camera Shake")]
    [SerializeField] private Toggle cameraShakeToggle;
    [SerializeField] private Slider cameraShakeSlider;
    private const string ShakeEnabledKey = "CameraShakeEnabled";
    private const string ShakeStrengthKey = "CameraShakeStrength";

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

        LoadCameraShakeSettings();

        cameraShakeToggle.onValueChanged.AddListener(
            OnCameraShakeToggleChanged
        );

        cameraShakeSlider.onValueChanged.AddListener(
            OnCameraShakeStrengthChanged
        );
    }

    private void ChangeBgmVolume(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    private void ChangeSfxVolume(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }

    private void OnDestroy()
    {
        bgmSlider.onValueChanged.RemoveListener(ChangeBgmVolume);
        sfxSlider.onValueChanged.RemoveListener(ChangeSfxVolume);
    }

    private void OnEnable()
    {
        if (cameraShakeToggle == null ||
            cameraShakeSlider == null)
        {
            Debug.LogError(
                "OptionUI에 카메라 흔들림 Toggle 또는 Slider가 연결되지 않았습니다.",
                this
            );

            return;
        }

        // 중복 등록 방지
        cameraShakeToggle.onValueChanged.RemoveListener(
            OnCameraShakeToggleChanged
        );

        cameraShakeSlider.onValueChanged.RemoveListener(
            OnCameraShakeStrengthChanged
        );

        cameraShakeToggle.onValueChanged.AddListener(
            OnCameraShakeToggleChanged
        );

        cameraShakeSlider.onValueChanged.AddListener(
            OnCameraShakeStrengthChanged
        );

        LoadCameraShakeSettings();
    }

    private void OnDisable()
    {
        if (cameraShakeToggle != null)
        {
            cameraShakeToggle.onValueChanged.RemoveListener(
                OnCameraShakeToggleChanged
            );
        }

        if (cameraShakeSlider != null)
        {
            cameraShakeSlider.onValueChanged.RemoveListener(
                OnCameraShakeStrengthChanged
            );
        }
    }

    private void LoadCameraShakeSettings()
    {
        bool enabled =
            PlayerPrefs.GetInt(ShakeEnabledKey, 1) == 1;

        float strength =
            PlayerPrefs.GetFloat(ShakeStrengthKey, 1f);

        cameraShakeToggle.SetIsOnWithoutNotify(enabled);
        cameraShakeSlider.SetValueWithoutNotify(strength);

        cameraShakeSlider.interactable = enabled;

        // 현재 카메라에도 불러온 값 적용
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.SetShakeEnabled(
                enabled,
                false
            );

            CameraShake.Instance.SetStrengthMultiplier(
                strength,
                false
            );
        }
    }

    private void OnCameraShakeToggleChanged(
        bool enabled)
    {
#if ENABLE_UNITYEVENTS
        Debug.Log($"카메라 흔들림 토글 변경: {enabled}");

        if (CameraShake.Instance == null)
        {
            Debug.LogError("CameraShake.Instance가 null입니다.");
            return;
        }
#endif
        CameraShake.Instance?
            .SetShakeEnabled(enabled);

        cameraShakeSlider.interactable = enabled;
    }

    private void OnCameraShakeStrengthChanged(
        float value)
    {
#if ENABLE_UNITYEVENTS
        Debug.Log($"카메라 흔들림 세기 변경: {value}");

        if (CameraShake.Instance == null)
        {
            Debug.LogError("CameraShake.Instance가 null입니다.");
            return;
        }
#endif
        CameraShake.Instance?
            .SetStrengthMultiplier(value);
    }
}