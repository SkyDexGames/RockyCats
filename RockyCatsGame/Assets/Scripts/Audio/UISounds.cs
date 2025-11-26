using UnityEngine;

[CreateAssetMenu(fileName = "UISounds", menuName = "Audio/UI Sounds")]
public class UISounds : ScriptableObject
{
    [Header("Button Click")]
    public AudioClip buttonClick;
    [Range(0f, 3f)] public float buttonClickVolume = 1f;

    [Header("Button Hover")]
    public AudioClip buttonHover;
    [Range(0f, 3f)] public float buttonHoverVolume = 1f;
}

