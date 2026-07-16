using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    public float BgmVolume => bgmSource.volume;
    public float SfxVolume => sfxSource.volume;

    [Header("BGM Clips")]
    [SerializeField] private List<BGMData> bgmList;

    [Header("SFX Clips")]
    [SerializeField] private List<SFXData> sfxList;

    private Dictionary<BGMType, AudioClip> bgmDictionary;
    private Dictionary<SFXType, AudioClip> sfxDictionary;

    private BGMType? currentBGM;

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateDictionaries();
        LoadVolume();
    }

    private void CreateDictionaries()
    {
        bgmDictionary = new Dictionary<BGMType, AudioClip>();

        foreach (BGMData data in bgmList)
        {
            if (data.clip == null)
                continue;

            if (!bgmDictionary.ContainsKey(data.type))
            {
                bgmDictionary.Add(data.type, data.clip);
            }
        }

        sfxDictionary = new Dictionary<SFXType, AudioClip>();

        foreach (SFXData data in sfxList)
        {
            if (data.clip == null)
                continue;

            if (!sfxDictionary.ContainsKey(data.type))
            {
                sfxDictionary.Add(data.type, data.clip);
            }
        }
    }

    public void PlayBgm(BGMType type)
    {
        if (!bgmDictionary.TryGetValue(type, out AudioClip clip))
        {
            Debug.LogWarning($"등록되지 않은 BGM입니다: {type}");
            return;
        }

        // 같은 음악이 이미 재생 중이면 다시 시작하지 않는다.
        if (currentBGM == type && bgmSource.isPlaying)
            return;

        currentBGM = type;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBgm()
    {
        bgmSource.Stop();
        currentBGM = null;
    }

    public void PlaySFX(SFXType type)
    {
        if (!sfxDictionary.TryGetValue(type, out AudioClip clip))
        {
            Debug.LogWarning($"등록되지 않은 SFX입니다: {type}");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(SFXType type, float volumeScale)
    {
        if (!sfxDictionary.TryGetValue(type, out AudioClip clip))
        {
            Debug.LogWarning($"등록되지 않은 SFX입니다: {type}");
            return;
        }

        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void SetBGMVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        bgmSource.volume = volume;
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    private void LoadVolume()
    {
        bgmSource.volume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 1f);
        sfxSource.volume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }
}

[Serializable]
public class BGMData
{
    public BGMType type;
    public AudioClip clip;
}

[Serializable]
public class SFXData
{
    public SFXType type;
    public AudioClip clip;
}