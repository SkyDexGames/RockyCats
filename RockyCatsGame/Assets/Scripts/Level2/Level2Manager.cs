using UnityEngine;
using Photon.Pun;
using TMPro;

public class Level2Manager : MonoBehaviourPun
{
    public static Level2Manager Instance;

    [Header("HUD Elements")]
    [SerializeField] private HUDElement[] hudElements; // Para reutilizar ShowHUD/HideHUD por nombre
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private string puzzleHUDName = "Level2HUD"; // Contenedor de HUD para este nivel

    [Header("Config (display)")]
    [SerializeField] private int totalRounds = 8; // Se puede actualizar en runtime desde el manager del puzzle

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // API de HUD (similar a Level1Manager para consistencia)
    public void ShowHUD(string hudName)
    {
        if (hudElements == null) return;
        for (int i = 0; i < hudElements.Length; i++)
        {
            if (hudElements[i].hudName == hudName)
            {
                hudElements[i].Show();
                return;
            }
        }
    }

    public void HideHUD(string hudName)
    {
        if (hudElements == null) return;
        for (int i = 0; i < hudElements.Length; i++)
        {
            if (hudElements[i].hudName == hudName)
            {
                hudElements[i].Hide();
                return;
            }
        }
    }

    // Métodos llamados por GasSequenceManager (en sus RPC hooks)
    public void SetTotalRounds(int rounds)
    {
        totalRounds = Mathf.Max(1, rounds);
    }

    public void OnBeginRound(int roundIndex, int length)
    {
        if (!string.IsNullOrEmpty(puzzleHUDName))
            ShowHUD(puzzleHUDName);

        SetRoundLabel(roundIndex, length);
        SetStatus("Observa la secuencia…");
    }

    public void OnBeginInput(int roundIndex)
    {
        SetStatus("Ingresa el patrón");
    }

    public void OnInputFeedback(bool correct, int roundIndex, int stepIndex, int buttonId)
    {
        if (correct)
            SetStatus("Correcto");
        else
            SetStatus("Incorrecto, repite la secuencia");
    }

    public void OnRoundSuccess(int roundIndex)
    {
        SetStatus($"Ronda {roundIndex + 1} completada");
    }

    public void OnRoundFail(int roundIndex)
    {
        SetStatus("Fallaste, se reinicia la ronda");
    }

    public void OnPuzzleCompleted()
    {
        SetStatus("Puzzle completado");
    }

    private void SetStatus(string text)
    {
        if (statusText != null)
        {
            statusText.text = text;
        }
    }

    private void SetRoundLabel(int roundIndex, int length)
    {
        if (roundText != null)
        {
            roundText.text = $"Ronda {roundIndex + 1}/{totalRounds} ({length})";
        }
    }
}

