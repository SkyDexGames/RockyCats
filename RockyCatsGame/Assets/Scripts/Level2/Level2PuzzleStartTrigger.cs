using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PhotonView))]
[DisallowMultipleComponent]
public class Level2PuzzleStartTrigger : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GasSequenceManager sequenceManager;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Comportamiento")]
    [SerializeField] private bool waitForBothPlayers = true;
    [SerializeField] private int cameraActivePriority = 20; // La prioridad de la VCam cuando se activa

    private PhotonView pv;
    private HashSet<int> playersInTrigger = new HashSet<int>();
    private bool fired = false;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fired) return;
        if (!other.CompareTag("Player")) return;

        PhotonView playerPV = other.GetComponent<PhotonView>();
        if (playerPV == null || !playerPV.IsMine) return; // Solo reacciona al jugador local

        if (waitForBothPlayers)
        {
            pv.RPC("RPC_PlayerEntered", RpcTarget.AllBuffered, playerPV.ViewID);
        }
        else
        {
            pv.RPC("RPC_BeginPuzzle", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_PlayerEntered(int viewId)
    {
        if (!playersInTrigger.Contains(viewId))
        {
            playersInTrigger.Add(viewId);
        }

        if (!fired && playersInTrigger.Count >= PhotonNetwork.PlayerList.Length)
        {
            pv.RPC("RPC_BeginPuzzle", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_BeginPuzzle()
    {
        if (fired) return;
        fired = true;

        // Cambiar a la cámara fija
        if (virtualCamera != null)
        {
            virtualCamera.Priority = cameraActivePriority;
        }

        // Iniciar el puzzle (solo lo hará el Master internamente)
        if (sequenceManager != null)
        {
            sequenceManager.StartPuzzleFromTrigger();
        }

        // Destruir el trigger para no re-disparar
        Destroy(gameObject);
    }
}

