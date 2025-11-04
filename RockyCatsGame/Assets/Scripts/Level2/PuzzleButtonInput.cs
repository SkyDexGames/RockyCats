using UnityEngine;
using Photon.Pun;

public class PuzzleButtonInput : MonoBehaviour
{
    [Range(0, 3)] public int buttonId = 0;
    [SerializeField] private GasSequenceManager sequenceManager;

    [Header("Feedback Visual")]
    [Tooltip("Duración del feedback visual al presionar el botón")]
    [SerializeField] private float feedbackDuration = 0.3f;

    void Awake()
    {
        if (sequenceManager == null)
            sequenceManager = FindObjectOfType<GasSequenceManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificar si es un jugador
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null || other.GetComponent<CharacterController>() != null)
        {
            Debug.Log($"[PuzzleButtonInput] Jugador tocó el botón {buttonId}");
            Press();
        }
    }

    public void Press()
    {
        if (sequenceManager == null)
        {
            Debug.LogWarning($"[PuzzleButtonInput] sequenceManager es NULL en botón {buttonId}!");
            return;
        }

        // Verificar si el puzzle está aceptando inputs
        if (!sequenceManager.CanAcceptInput)
        {
            Debug.Log($"[PuzzleButtonInput] Botón {buttonId} presionado pero el puzzle no está aceptando inputs (mostrando patrón o inactivo)");
            return;
        }

        Debug.Log($"[PuzzleButtonInput] Botón {buttonId} presionado - Enviando input");

        // Activar el cráter correspondiente como feedback visual
        sequenceManager.ActivateCraterFeedback(buttonId, feedbackDuration);

        // Enviar el input al manager
        sequenceManager.SubmitInput(buttonId);
    }
}