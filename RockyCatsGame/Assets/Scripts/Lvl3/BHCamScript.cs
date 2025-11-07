using Photon.Pun;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;

public class BHCamScript : MonoBehaviourPunCallbacks
{
    public CinemachineVirtualCamera cameraToSwitchTo;
    public LevelManager levelManager;

    // Se guarda solo en el Master quiénes han entrado
    private HashSet<int> playersInside = new HashSet<int>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView playerPhotonView = other.GetComponent<PhotonView>();
        if (playerPhotonView == null) return;

        // Cambia cámara solo en el jugador local
        if (playerPhotonView.IsMine)
        {
            CameraManager.Instance.SwitchCamera(cameraToSwitchTo);
        }

        // Avisamos al MasterClient que este jugador entró
        photonView.RPC("RPC_PlayerEnteredTrigger", RpcTarget.MasterClient, playerPhotonView.OwnerActorNr);
    }

    [PunRPC]
    private void RPC_PlayerEnteredTrigger(int actorNumber)
    {
        // Si ya estaba registrado, no lo contamos otra vez
        playersInside.Add(actorNumber);

        CheckAllPlayersInside();
    }

    private void CheckAllPlayersInside()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int totalPlayers = PhotonNetwork.PlayerList.Length;

        Debug.Log($"Jugadores dentro: {playersInside.Count}/{totalPlayers}");

        if (playersInside.Count == totalPlayers)
        {
            Debug.Log("Todos los jugadores entraron al trigger. Lanzando waves...");
            levelManager.LaunchWaves();
            photonView.RPC("RPC_ActiveScores", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_ActiveScores()
    {
        HUDManager.Instance.ShowHUD("BHScores");
    }
}
