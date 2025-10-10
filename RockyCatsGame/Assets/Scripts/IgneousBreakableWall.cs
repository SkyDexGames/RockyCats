using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IgneousBreakableWall : MonoBehaviour, IIgneousInteractable
{
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void OnIgneousCollision()
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("RPC_BreakWall", RpcTarget.AllBuffered);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [PunRPC]
    void RPC_BreakWall()
    {
        Destroy(gameObject);
    }
}