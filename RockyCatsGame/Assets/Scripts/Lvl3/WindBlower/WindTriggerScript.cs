using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WindTriggerScript : MonoBehaviour
{
    public float windForce = 3.3f; //es el sweet spot que encontre a punta de testing
    public Vector3 windDirection = Vector3.forward;
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.SetExternalVelocity(
                        windDirection * windForce, 
                        0.15f
                    );
                }
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.SetExternalVelocity(Vector3.zero, 0f);
                }
            }
        }
    }
}