using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Photon.Realtime;

public class Level2Manager : MonoBehaviourPunCallbacks
{
    public static Level2Manager Instance;
    
    [SerializeField] private HUDElement[] hudElements;

    [Header("HUD Elements")]
    [SerializeField] private GameObject roundTextContainer; // Contenedor del texto de rondas (para ocultar/mostrar)
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private GameObject statusTextContainer; // Contenedor del texto de status (para ocultar/mostrar)
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Barra de Energía")]
    [SerializeField] private GameObject energyBarContainer; // Contenedor completo de la barra (se oculta/muestra)
    [SerializeField] private Image energyBarFill; 
    [Tooltip("Duración de la animación de llenado de la barra")]
    [SerializeField] private float fillAnimationDuration = 0.5f;

    [Header("Config (display)")]
    [SerializeField] private int totalRounds = 8; // Se puede actualizar en runtime desde el manager del puzzle

    private float currentEnergyFill = 0f; // Valor actual de la barra (0 a 1)
    private Coroutine fillCoroutine;

    private APIRequests apiRequests;
    private bool isPaused = false;

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

        // Ocultar todos los elementos del HUD al inicio (solo se muestran durante el puzzle)
        if (roundTextContainer != null)
        {
            roundTextContainer.SetActive(false);
        }

        if (statusTextContainer != null)
        {
            statusTextContainer.SetActive(false);
        }

        if (energyBarContainer != null)
        {
            energyBarContainer.SetActive(false);
        }
    }

    // Métodos llamados por GasSequenceManager (en sus RPC hooks)
    public void SetTotalRounds(int rounds)
    {
        totalRounds = Mathf.Max(1, rounds);
    }

    public void OnBeginRound(int roundIndex, int length)
    {
        // Mostrar todos los elementos del HUD cuando empieza el puzzle
        if (roundTextContainer != null)
        {
            roundTextContainer.SetActive(true);
        }

        if (statusTextContainer != null)
        {
            statusTextContainer.SetActive(true);
        }

        if (energyBarContainer != null)
        {
            energyBarContainer.SetActive(true);
        }

        SetRoundLabel(roundIndex, length);
        SetStatus("watch the sequence...");
    }

    public void OnBeginInput(int roundIndex)
    {
        SetStatus("enter Pattern");
    }

    public void OnInputFeedback(bool correct, int roundIndex, int stepIndex, int buttonId)
    {
        // No mostrar feedback inmediato - solo actualizar el contador de progreso
        SetStatus($"entering Pattern ({stepIndex + 1})");
    }

    public void OnRoundSuccess(int roundIndex)
    {
        SetStatus($"round {roundIndex + 1} complete!");

        // Actualizar la barra de energía
        UpdateEnergyBar(roundIndex + 1); // +1 porque roundIndex es 0-based
    }

    public void OnRoundFail(int roundIndex)
    {
        SetStatus("round failed, trying again");
        // No actualizar la barra cuando falla
    }

    public void OnPuzzleCompleted()
    {
        SetStatus("Puzzle Completed");
        //Debug.Log("[Level2Manager] Puzzle completado!");
        //Debug.Log($"[Level2Manager] {PlayerPrefs.GetString("PlayerUsername", "N/A")}");

        // Asegurar que la barra esté completamente llena
        UpdateEnergyBar(totalRounds);
        if (PhotonNetwork.IsMasterClient && PlayerPrefs.HasKey("PlayerUsername"))
        {
            if (PlayerPrefs.GetInt("PlayerLevels") < 2)
            {
                //Debug.Log("[Level2Manager] Actualizando niveles del jugador en el servidor...");
                apiRequests = new APIRequests();
                string username = PlayerPrefs.GetString("PlayerUsername");
                StartCoroutine(apiRequests.UpdatePlayerLevels(username, 2,
                    onSuccess: () =>
                    {
                        //Debug.Log("Niveles del jugador actualizados correctamente.");
                        PlayerPrefs.SetInt("PlayerLevels", 2);
                    },
                    onError: (error) =>
                    {
                        Debug.LogError("Error al actualizar niveles del jugador: " + error);
                    }
                ));
            }
        }
        
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
            roundText.text = $"Round {roundIndex + 1}/{totalRounds} ({length})";
        }
    }

    /// Oculta todos los elementos del HUD del puzzle (Round, Status, Energy Bar)
    public void HideAllPuzzleHUD()
    {
        if (roundTextContainer != null)
        {
            roundTextContainer.SetActive(false);
        }

        if (statusTextContainer != null)
        {
            statusTextContainer.SetActive(false);
        }

        if (energyBarContainer != null)
        {
            energyBarContainer.SetActive(false);
        }
    }
    
    //funciones mas generales para hacer handling de todo lo del hud, deberiamos hacer UN solo manager.

    public void ShowHUD(string hudName)
    {
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
        for (int i = 0; i < hudElements.Length; i++)
        {
            if (hudElements[i].hudName == hudName)
            {
                hudElements[i].Hide();
                return;
            }
        }
    }

    /// Resetea la barra de energía a 0
    private void ResetEnergyBar()
    {
        currentEnergyFill = 0f;
        if (energyBarFill != null)
        {
            energyBarFill.fillAmount = 0f;
        }
    }

    /// Actualiza la barra de energía basándose en las rondas completadas
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

    /// Anima el llenado de la barra de energía
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
    public void LeaveMatch()
    {
        Debug.Log("Leaving match...");
        
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void PauseGame()
    {
        ShowHUD("PauseMenu");

        if(PhotonNetwork.IsMasterClient)
            ShowHUD("QuitToMap");
    }
    

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    public void QuitToMap()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_LoadScene", RpcTarget.All, 1);
        }
    }
    public void TogglePause()
    {
        isPaused = !isPaused; 
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
        
    }

    public void ResumeGame()
    {
        HideHUD("PauseMenu");
    }

    [PunRPC]
    void RPC_LoadScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    

    
}

