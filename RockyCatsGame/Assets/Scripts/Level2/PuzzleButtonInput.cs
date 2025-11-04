using UnityEngine;
using Photon.Pun;

public class PuzzleButtonInput : MonoBehaviour
{
    [Range(0, 3)] public int buttonId = 0;
    [SerializeField] private GasSequenceManager sequenceManager;

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

        Debug.Log($"[PuzzleButtonInput] Botón {buttonId} presionado - Enviando input");
        sequenceManager.SubmitInput(buttonId);
    }
}