using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SectionTrigger : MonoBehaviour
{
    public GameObject roadSection;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
        
            if (playerPhotonView.IsMine && PhotonNetwork.IsMasterClient)
            {
                Transform granny = transform.parent.parent;
                
                GameObject newSection = Instantiate(roadSection, granny);
                
                newSection.transform.localPosition = Vector3.zero;
                newSection.transform.localRotation = Quaternion.identity;
                
                newSection.transform.localPosition = new Vector3(-80, 0.7f, 190);
            }
        }
    }
}