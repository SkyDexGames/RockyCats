using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSFX", menuName = "Audio/Character SFX")]
public class CharacterSFX : ScriptableObject
{
    [Header("Movement")]
    public AudioClip[] walkClips;
    public AudioClip jumpClip;

    [Header("Abilities")]
    public AudioClip dashClip;

    [Header("Status")]
    public AudioClip hurtClip;
    public AudioClip deathClip;

    [Header("Settings")]
    [Range(0.1f, 0.5f)] public float footstepInterval = 0.3f;

    // Getters para clips aleatorios (variación en pasos)
    public AudioClip GetRandomWalkClip()
    {
        if (walkClips == null || walkClips.Length == 0) return null;
        return walkClips[Random.Range(0, walkClips.Length)];
    }
}

