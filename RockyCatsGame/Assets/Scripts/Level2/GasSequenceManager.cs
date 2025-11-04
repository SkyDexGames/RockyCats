using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class GasSequenceManager : MonoBehaviourPunCallbacks
{
    [Header("Referencias")]
    [SerializeField] private GasSequenceConfig config;
    [SerializeField] private GasCraterController[] craters; // 0..3

    [Header("Inicio")]
    [SerializeField] private bool autoStartOnStart = false; // Si true, el Master inicia automáticamente en Start()

    [Header("Cámara (opcional)")]
    [SerializeField] private CinemachineVirtualCamera puzzleCamera; // Cámara fija del puzzle para bajar prioridad al terminar
    [SerializeField] private int cameraPriorityAfterPuzzle = 5;


    private enum PuzzleState { Idle, ShowingSequence, AwaitingInputs, Completed }
    private PuzzleState state = PuzzleState.Idle;

    private List<int> currentSequence = new List<int>();
    private int currentRoundIndex = 0; // 0..7
    private int currentStepIndex = 0;  // índice del input esperado

    private System.Random rng;

    void Start()
    {
        if (config == null)
        {
            Debug.LogWarning("[GasSequenceManager] Falta GasSequenceConfig en el inspector.");
        }

        // Inicio automtico opcional
        if (PhotonNetwork.IsMasterClient && autoStartOnStart)
        {
            EnsureSeedInitialized();
            StartRound(0);
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (state == PuzzleState.Completed) return;
        if (PhotonNetwork.IsMasterClient)
        {
            // Reiniciar la ronda actual para consistencia
            if (rng == null)
            {
                int seed = (config != null && config.randomSeed != 0) ? config.randomSeed : UnityEngine.Random.Range(1, int.MaxValue);
                rng = new System.Random(seed);
                photonView.RPC("RPC_SetSeed", RpcTarget.Others, seed);
            }
            RestartCurrentRound();
        }
    }

    [PunRPC]
    void RPC_SetSeed(int seed)
    {
        rng = new System.Random(seed);
    }

    private void EnsureSeedInitialized()
    {
        if (rng == null)
        {
            int seed = (config != null && config.randomSeed != 0) ? config.randomSeed : UnityEngine.Random.Range(1, int.MaxValue);
            rng = new System.Random(seed);
            photonView.RPC("RPC_SetSeed", RpcTarget.Others, seed);
        }
    }

    [PunRPC]
    void RPC_RequestStartPuzzle()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        EnsureSeedInitialized();
        StartRound(0);
    }

    // Punto de entrada llamado por el trigger de Level 2
    public void StartPuzzleFromTrigger()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            EnsureSeedInitialized();
            StartRound(0);
        }
        else
        {
            photonView.RPC("RPC_RequestStartPuzzle", RpcTarget.MasterClient);
        }
    }


    // Llamado por botones locales
    public void SubmitInput(int buttonId)
    {
        // Enviar al Master para validación
        photonView.RPC("RPC_SubmitInput", RpcTarget.MasterClient, buttonId, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    void RPC_SubmitInput(int buttonId, int actorNumber, PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (state != PuzzleState.AwaitingInputs) return;
        if (craters == null || craters.Length == 0) return;
        if (currentSequence == null || currentSequence.Count == 0) return;

        bool correct = (buttonId == currentSequence[currentStepIndex]);
        photonView.RPC("RPC_InputFeedback", RpcTarget.All, correct, currentRoundIndex, currentStepIndex, buttonId);

        if (correct)
        {
            currentStepIndex++;
            if (currentStepIndex >= currentSequence.Count)
            {
                // Ronda superada
                photonView.RPC("RPC_RoundSuccess", RpcTarget.All, currentRoundIndex);
                StartNextRoundOrComplete();
            }
        }
        else
        {
            // Falló: reiniciar ronda actual
            photonView.RPC("RPC_RoundFail", RpcTarget.All, currentRoundIndex);
            RestartCurrentRound();
        }
    }

    void StartNextRoundOrComplete()
    {
        currentRoundIndex++;
        if (currentRoundIndex >= (config != null ? config.rounds : 8))
        {
            photonView.RPC("RPC_PuzzleCompleted", RpcTarget.All);
        }
        else
        {
            StartRound(currentRoundIndex);
        }
    }

    void StartRound(int roundIndex)
    {
        StopAllCoroutines();
        state = PuzzleState.ShowingSequence;
        currentStepIndex = 0;

        // Generar secuencia con posibles repeticiones
        int length = (config != null) ? config.GetSequenceLength(roundIndex) : Mathf.Min(4 + roundIndex, 8);
        int craterCount = (config != null) ? config.craterCount : 4;
        currentSequence.Clear();
        for (int i = 0; i < length; i++)
        {
            int id = rng != null ? rng.Next(0, craterCount) : Random.Range(0, craterCount);
            currentSequence.Add(id);
        }

        // Notificar inicio de ronda
        photonView.RPC("RPC_BeginRound", RpcTarget.All, roundIndex, length);
        StartCoroutine(CoShowSequence(roundIndex, currentSequence));
    }

    void RestartCurrentRound()
    {
        StartRound(currentRoundIndex);
    }

    private IEnumerator CoShowSequence(int roundIndex, List<int> sequence)
    {
        float stepInterval = (config != null) ? config.GetStepInterval(roundIndex) : Mathf.Max(0.5f, 1.2f - 0.1f * roundIndex);
        float gasDuration = (config != null) ? config.gasDuration : 0.5f;

        yield return new WaitForSeconds(0.6f); // pequeña pausa antes de mostrar

        for (int i = 0; i < sequence.Count; i++)
        {
            int id = sequence[i];
            photonView.RPC("RPC_EmitCrater", RpcTarget.All, id, gasDuration);
            yield return new WaitForSeconds(stepInterval);
        }

        photonView.RPC("RPC_BeginInput", RpcTarget.All, roundIndex);
        state = PuzzleState.AwaitingInputs;
        currentStepIndex = 0;
    }

    [PunRPC]
    void RPC_EmitCrater(int craterId, float duration)
    {
        if (craters == null || craterId < 0 || craterId >= craters.Length) return;
        craters[craterId].PlayGas(duration);
    }

    [PunRPC]
    void RPC_BeginRound(int roundIndex, int length)
    {
        var l2 = Level2Manager.Instance;
        if (l2 != null)
        {
            if (config != null) l2.SetTotalRounds(config.rounds);
            l2.OnBeginRound(roundIndex, length);
        }
    }

    [PunRPC]
    void RPC_BeginInput(int roundIndex)
    {
        var l2 = Level2Manager.Instance;
        if (l2 != null)
        {
            l2.OnBeginInput(roundIndex);
        }
    }

    [PunRPC]
    void RPC_InputFeedback(bool correct, int roundIndex, int stepIndex, int buttonId)
    {
        var l2 = Level2Manager.Instance;
        if (l2 != null)
        {
            l2.OnInputFeedback(correct, roundIndex, stepIndex, buttonId);
        }
    }

    [PunRPC]
    void RPC_RoundSuccess(int roundIndex)
    {
        state = PuzzleState.Idle; // pausa corta antes de la siguiente
        var l2 = Level2Manager.Instance;
        if (l2 != null)
        {
            l2.OnRoundSuccess(roundIndex);
        }
    }

    [PunRPC]
    void RPC_RoundFail(int roundIndex)
    {
        state = PuzzleState.Idle; // se reinicia y se muestra de nuevo
        var l2 = Level2Manager.Instance;
        if (l2 != null)
        {
            l2.OnRoundFail(roundIndex);
        }
    }

    [PunRPC]
    void RPC_PuzzleCompleted()
    {
        state = PuzzleState.Completed;

        var l2 = Level2Manager.Instance;
        if (l2 != null)
        {
            l2.OnPuzzleCompleted();
        }

        // Volver a la cámara del jugador: bajar la prioridad de la cámara del puzzle si está asignada
        if (puzzleCamera != null)
        {
            puzzleCamera.Priority = cameraPriorityAfterPuzzle;
        }

        // Disparar cutscene usando VideoManager del nivel si existe
        var vm = FindObjectOfType<VideoManager>();
        if (vm != null && vm.photonView != null)
        {
            vm.photonView.RPC("RPC_PlayVideoForAll", RpcTarget.All);
        }
        else
        {
            Debug.LogWarning("[GasSequenceManager] No se encontró VideoManager en la escena para reproducir la cutscene.");
        }
    }
}

