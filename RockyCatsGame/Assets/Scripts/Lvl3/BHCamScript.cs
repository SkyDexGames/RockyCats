using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Cinemachine;
using UnityEngine;

public class BHCamScript : MonoBehaviour
{
    public CinemachineVirtualCamera cameraToSwitchTo;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            if(playerPhotonView.IsMine)
            {
                CameraManager.Instance.SwitchCamera(cameraToSwitchTo);
            }
        }
    }
}
