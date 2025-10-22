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
                
                GameObject newSection = PhotonNetwork.Instantiate("PhotonPrefabs/Level1/SurfingTunnel", Vector3.zero, Quaternion.identity);
                newSection.transform.SetParent(granny);
                newSection.transform.localPosition = new Vector3(-80, 0.7f, 180);
                newSection.transform.localScale = Vector3.one * 0.5f;
            }
        }
    }
}