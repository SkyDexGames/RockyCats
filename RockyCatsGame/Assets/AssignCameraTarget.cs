using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class AssignCameraTarget : MonoBehaviour
{
    void Start()
    {
        PhotonView photonView = GetComponent<PhotonView>();
        if(photonView != null && photonView.IsMine)
        {
            //find cam by tag
            GameObject cameraObject = GameObject.FindGameObjectWithTag("CMVirtualCam");
            if(cameraObject != null)
            {
                CinemachineVirtualCamera virtualCam = cameraObject.GetComponent<CinemachineVirtualCamera>();
                if(virtualCam != null)
                {
                    virtualCam.Follow = this.transform;
                    //virtualCam.LookAt = this.transform;
                    // le quitamos el LookAt para que no rote la camara
                }
            }
        }
    }
}
