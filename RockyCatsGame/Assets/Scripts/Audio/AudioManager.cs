using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 1f;

    [Header("Level Audio Configs")]
    [SerializeField] private LevelAudioConfig[] levelConfigs;

    [Header("UI Sounds")]
    [SerializeField] private UISounds uiSounds;

    [Header("BGM Transition")]
    [SerializeField] private float fadeTime = 1f;

    private LevelAudioConfig currentLevelConfig;
    private Coroutine bgmFadeCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void InitializeAudioSources()
    {
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        if (uiSource == null)
        {
            uiSource = gameObject.AddComponent<AudioSource>();
            uiSource.loop = false;
            uiSource.playOnAwake = false;
        }

        LoadVolumeSettings();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LevelAudioConfig config = GetConfigForScene(scene.buildIndex);
        if (config != null)
        {
            PlayLevelBGM(config);
        }
    }

    LevelAudioConfig GetConfigForScene(int sceneIndex)
    {
        foreach (var config in levelConfigs)
        {
            if (config != null && config.sceneIndex == sceneIndex)
                return config;
        }
        return null;
    }

    void PlayLevelBGM(LevelAudioConfig config)
    {
        // Si el clip es el mismo que ya está sonando, no reiniciar
        if (config.bgmClip != null && bgmSource.clip == config.bgmClip && bgmSource.isPlaying)
        {
            currentLevelConfig = config;
            return;
        }

        currentLevelConfig = config;

        if (config.bgmClip != null)
        {
            if (bgmFadeCoroutine != null)
                StopCoroutine(bgmFadeCoroutine);

            bgmFadeCoroutine = StartCoroutine(CrossFadeBGM(config.bgmClip));
        }
    }

    IEnumerator CrossFadeBGM(AudioClip newClip)
    {
        float startVolume = bgmSource.volume;

        // Fade out
        while (bgmSource.volume > 0)
        {
            bgmSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        // Fade in
        while (bgmSource.volume < bgmVolume * masterVolume)
        {
            bgmSource.volume += bgmVolume * masterVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        bgmSource.volume = bgmVolume * masterVolume;
    }

    // Public methods for playing sounds
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    public void PlaySFX(AudioClip clip, float volumeMultiplier)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume * volumeMultiplier);
    }

    public void PlayUI(AudioClip clip)
    {
        if (clip != null)
            uiSource.PlayOneShot(clip, uiVolume * masterVolume);
    }

    public void PlayUI(AudioClip clip, float volumeMultiplier)
    {
        if (clip != null)
            uiSource.PlayOneShot(clip, uiVolume * masterVolume * volumeMultiplier);
    }

    // UI Sound shortcuts
    public void PlayButtonClick()
    {
        if (uiSounds != null)
            PlayUI(uiSounds.buttonClick, uiSounds.buttonClickVolume);
    }

    public void PlayButtonHover()
    {
        if (uiSounds != null)
            PlayUI(uiSounds.buttonHover, uiSounds.buttonHoverVolume);
    }

    // BGM control
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        StartCoroutine(CrossFadeBGM(clip));
    }

    public void StopBGM()
    {
        StartCoroutine(FadeOutBGM());
    }

    IEnumerator FadeOutBGM()
    {
        float startVolume = bgmSource.volume;

        while (bgmSource.volume > 0)
        {
            bgmSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = null;
    }

    // Volume control
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        SaveVolumeSettings();
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume * masterVolume;
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        SaveVolumeSettings();
    }

    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
        SaveVolumeSettings();
    }

    public float GetSFXVolume()
    {
        return sfxVolume * masterVolume;
    }

    public LevelSFXEntry GetLevelSFXEntry(string key)
    {
        if (currentLevelConfig == null) return null;
        return currentLevelConfig.GetSFXEntry(key);
    }

    void UpdateAllVolumes()
    {
        bgmSource.volume = bgmVolume * masterVolume;
    }

    void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("UIVolume", uiVolume);
        PlayerPrefs.Save();
    }

    void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);
    }

    // Play level-specific SFX by key (uses volume from config)
    public void PlayLevelSFX(string key)
    {
        if (currentLevelConfig == null) return;

        LevelSFXEntry entry = currentLevelConfig.GetSFXEntry(key);
        if (entry != null && entry.clip != null)
            PlaySFX(entry.clip, entry.volume);
    }

    public void PlayLevelSFX(string key, float volumeMultiplier)
    {
        if (currentLevelConfig == null) return;

        LevelSFXEntry entry = currentLevelConfig.GetSFXEntry(key);
        if (entry != null && entry.clip != null)
            PlaySFX(entry.clip, entry.volume * volumeMultiplier);
    }

    // Getters
    public UISounds GetUISounds() => uiSounds;
    public LevelAudioConfig GetCurrentLevelConfig() => currentLevelConfig;
}

