using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelAudioConfig", menuName = "Audio/Level Audio Config")]
public class LevelAudioConfig : ScriptableObject
{
    [Header("Scene Settings")]
    public int sceneIndex;
    public string sceneName;

    [Header("Background Music")]
    public AudioClip bgmClip;
    [Range(0f, 1f)] public float bgmVolume = 1f;

    [Header("Ambient Sounds")]
    public AudioClip[] ambientClips;
    [Range(0f, 1f)] public float ambientVolume = 0.5f;

    [Header("Level Specific SFX")]
    public LevelSFXEntry[] levelSFX;

    // Get SFX by key name
    public AudioClip GetSFX(string key)
    {
        foreach (var entry in levelSFX)
        {
            if (entry.key == key)
                return entry.clip;
        }
        return null;
    }

    // Get SFX entry by key (includes volume)
    public LevelSFXEntry GetSFXEntry(string key)
    {
        foreach (var entry in levelSFX)
        {
            if (entry.key == key)
                return entry;
        }
        return null;
    }

    // Get random SFX from a key (if you have multiple variations)
    public AudioClip GetRandomSFX(string key)
    {
        var matches = Array.FindAll(levelSFX, e => e.key == key);
        if (matches.Length == 0) return null;
        return matches[UnityEngine.Random.Range(0, matches.Length)].clip;
    }
}

[Serializable]
public class LevelSFXEntry
{
    [Tooltip("Nombre identificador del SFX (ej: lava_bubble, door_open, explosion)")]
    public string key;
    public AudioClip clip;
    [Range(0f, 3f)] public float volume = 1f;
}

