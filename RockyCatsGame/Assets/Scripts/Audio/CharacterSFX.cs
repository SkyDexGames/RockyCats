using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSFX", menuName = "Audio/Character SFX")]
public class CharacterSFX : ScriptableObject
{
    [Header("Movement")]
    public AudioClip[] walkClips;
    [Range(0f, 2f)] public float walkVolume = 1f;
    public AudioClip jumpClip;
    [Range(0f, 2f)] public float jumpVolume = 1f;

    [Header("Abilities")]
    public AudioClip dashClip;
    [Range(0f, 2f)] public float dashVolume = 1f;

    [Header("Status")]
    public AudioClip hurtClip;
    [Range(0f, 2f)] public float hurtVolume = 1f;
    public AudioClip deathClip;
    [Range(0f, 2f)] public float deathVolume = 1f;

    [Header("Settings")]
    [Range(0.1f, 0.5f)] public float footstepInterval = 0.3f;

    // Getters para clips aleatorios (variación en pasos)
    public AudioClip GetRandomWalkClip()
    {
        if (walkClips == null || walkClips.Length == 0) return null;
        return walkClips[Random.Range(0, walkClips.Length)];
    }
}

