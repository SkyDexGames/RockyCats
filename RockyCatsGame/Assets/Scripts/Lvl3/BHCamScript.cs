using Photon.Pun;
using Cinemachine;
using UnityEngine;

public class BHCamScript : MonoBehaviourPunCallbacks
{
    public CinemachineVirtualCamera cameraToSwitchTo;
    public LevelManager levelManager;

    private bool chilliInside = false;
    private bool gizmoInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView playerPhotonView = other.GetComponent<PhotonView>();
        if (playerPhotonView == null) return;

        // Solo el jugador local cambia la cámara
        if (playerPhotonView.IsMine)
        {
            CameraManager.Instance.SwitchCamera(cameraToSwitchTo);

            // Avisamos al MasterClient que este jugador entró al trigger
            photonView.RPC("RPC_PlayerEnteredTrigger", RpcTarget.MasterClient, PhotonNetwork.IsMasterClient);
        }
    }

    [PunRPC]
    private void RPC_PlayerEnteredTrigger(bool isMaster)
    {
        // Este código se ejecuta solo en el MasterClient
        if (isMaster)
        {
            gizmoInside = true;
            Debug.Log(" Gizmo (Master) entró al trigger");
        }
        else
        {
            chilliInside = true;
            Debug.Log("Chilli (Cliente) entró al trigger");
        }

        // Si ambos están dentro, el Master ejecuta la acción
        if (gizmoInside && chilliInside)
        {
            Debug.Log("Ambos jugadores dentro, lanzando olas...");
            levelManager.LaunchWaves();

            // Reiniciamos el estado
            gizmoInside = false;
            chilliInside = false;

            // (Opcional) Sincronizamos con todos si quieres que vean el cambio
            photonView.RPC("RPC_ResetTriggerState", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_ResetTriggerState()
    {
        gizmoInside = false;
        chilliInside = false;
    }
}
