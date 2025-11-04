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

    // Llama esto desde UI/Button, OnMouseDown, o un trigger interactivo
    public void Press()
    {
        if (sequenceManager == null) return;
        sequenceManager.SubmitInput(buttonId);
    }
}

