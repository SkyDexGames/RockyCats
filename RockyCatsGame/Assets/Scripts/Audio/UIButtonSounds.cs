using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Override Sounds (optional)")]
    [SerializeField] private AudioClip customClickSound;
    [SerializeField] private AudioClip customHoverSound;

    [Header("Settings")]
    [SerializeField] private bool playHoverSound = true;
    [SerializeField] private bool playClickSound = true;

    private UISounds uiSounds;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            uiSounds = AudioManager.Instance.GetUISounds();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playHoverSound) return;
        if (AudioManager.Instance == null) return;

        AudioClip clip = customHoverSound != null ? customHoverSound : uiSounds?.buttonHover;
        if (clip != null)
        {
            AudioManager.Instance.PlayUI(clip);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!playClickSound) return;
        if (AudioManager.Instance == null) return;

        AudioClip clip = customClickSound != null ? customClickSound : uiSounds?.buttonClick;
        if (clip != null)
        {
            AudioManager.Instance.PlayUI(clip);
        }
    }
}

