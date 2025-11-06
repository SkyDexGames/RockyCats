using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeathTrigger : MonoBehaviourPunCallbacks
{
    public LevelManager levelManager; // Asigna desde el inspector

    public int playersInside = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playersInside++;
        CheckPlayers();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playersInside--;
        CheckPlayers();
    }

    private void CheckPlayers()
    {
        
        if (!PhotonNetwork.IsMasterClient) return;

        int totalPlayers = PhotonNetwork.PlayerList.Length;

        if (playersInside >= totalPlayers)
        {
            // Llama al método de fin de nivel en todos
            photonView.RPC("EndLevelRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void EndLevelRPC()
    {
        levelManager.EndLevel(false);
    }
}
