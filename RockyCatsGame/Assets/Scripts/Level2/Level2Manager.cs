using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Level2Manager : MonoBehaviourPun
{
    public static Level2Manager Instance;

    [Header("HUD Elements")]
    [SerializeField] private HUDElement[] hudElements; // Para reutilizar ShowHUD/HideHUD por nombre
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private string puzzleHUDName = "Level2HUD"; // Contenedor de HUD para este nivel

    [Header("Barra de Energía")]
    [SerializeField] private GameObject energyBarContainer; // Contenedor completo de la barra (se oculta/muestra)
    [SerializeField] private Image energyBarFill; // La imagen que se llenará (debe tener Image Type = Filled)
    [Tooltip("Duración de la animación de llenado de la barra")]
    [SerializeField] private float fillAnimationDuration = 0.5f;

    [Header("Config (display)")]
    [SerializeField] private int totalRounds = 8; // Se puede actualizar en runtime desde el manager del puzzle

    private float currentEnergyFill = 0f; // Valor actual de la barra (0 a 1)
    private Coroutine fillCoroutine;

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

    void Start()
    {
        // Inicializar la barra de energía vacía y oculta
        ResetEnergyBar();

        // Ocultar la barra al inicio
        if (energyBarContainer != null)
        {
            energyBarContainer.SetActive(false);
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

        // Mostrar la barra de energía cuando empieza el puzzle
        if (energyBarContainer != null)
        {
            energyBarContainer.SetActive(true);
        }

        SetRoundLabel(roundIndex, length);
        SetStatus("Observa la secuencia…");
    }

    public void OnBeginInput(int roundIndex)
    {
        SetStatus("Ingresa el patrón");
    }

    public void OnInputFeedback(bool correct, int roundIndex, int stepIndex, int buttonId)
    {
        // No mostrar feedback inmediato - solo actualizar el contador de progreso
        SetStatus($"Ingresando patrón ({stepIndex + 1})");
    }

    public void OnRoundSuccess(int roundIndex)
    {
        SetStatus($"Ronda {roundIndex + 1} completada");

        // Actualizar la barra de energía
        UpdateEnergyBar(roundIndex + 1); // +1 porque roundIndex es 0-based
    }

    public void OnRoundFail(int roundIndex)
    {
        SetStatus("Fallaste, se reinicia la ronda");
        // No actualizar la barra cuando falla
    }

    public void OnPuzzleCompleted()
    {
        SetStatus("Puzzle completado");

        // Asegurar que la barra esté completamente llena
        UpdateEnergyBar(totalRounds);
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

    /// <summary>
    /// Resetea la barra de energía a 0
    /// </summary>
    private void ResetEnergyBar()
    {
        currentEnergyFill = 0f;
        if (energyBarFill != null)
        {
            energyBarFill.fillAmount = 0f;
        }
    }

    /// <summary>
    /// Actualiza la barra de energía basándose en las rondas completadas
    /// </summary>
    /// <param name="completedRounds">Número de rondas completadas (1-based)</param>
    private void UpdateEnergyBar(int completedRounds)
    {
        if (energyBarFill == null)
        {
            Debug.LogWarning("[Level2Manager] energyBarFill no está asignado!");
            return;
        }

        // Calcular el fill target basado en las rondas completadas
        float targetFill = Mathf.Clamp01((float)completedRounds / totalRounds);

        // Detener cualquier animación anterior
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }

        // Iniciar la animación de llenado
        fillCoroutine = StartCoroutine(AnimateFillBar(targetFill));
    }

    /// <summary>
    /// Anima el llenado de la barra de energía
    /// </summary>
    private IEnumerator AnimateFillBar(float targetFill)
    {
        float startFill = currentEnergyFill;
        float elapsed = 0f;

        while (elapsed < fillAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fillAnimationDuration;

            // Interpolación suave
            currentEnergyFill = Mathf.Lerp(startFill, targetFill, t);
            energyBarFill.fillAmount = currentEnergyFill;

            yield return null;
        }

        // Asegurar que llegue exactamente al valor final
        currentEnergyFill = targetFill;
        energyBarFill.fillAmount = targetFill;

        fillCoroutine = null;
    }
}

