using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    private const string BgmVolumeKey = "BGMVolume";
    private const string SfxVolumeKey = "SFXVolume";

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float bgmVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    public float BgmVolume => bgmVolume;
    public float SfxVolume => sfxVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadVolume();
        ApplyVolume();
    }

    public void PlayBgm(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        // 이미 같은 음악이 재생 중이면 다시 시작하지 않는다.
        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBgm()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }

    public void PauseBgm()
    {
        bgmSource.Pause();
    }

    public void ResumeBgm()
    {
        bgmSource.UnPause();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;

        PlayerPrefs.SetFloat(BgmVolumeKey, bgmVolume);
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;

        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        bgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
    }

    private void ApplyVolume()
    {
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }
}