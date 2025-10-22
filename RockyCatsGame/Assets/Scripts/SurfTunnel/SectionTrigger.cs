using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SectionTrigger : MonoBehaviour
{
    private static int sectionSeedCounter = 1000;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                Transform granny = transform.parent.parent;
                
                GameObject roadSection = Resources.Load<GameObject>("SurfingTunnel");
                GameObject newSection = Instantiate(roadSection, granny);
                
                newSection.transform.localPosition = new Vector3(-80, 0.7f, 180);

                SurfTunnelManager manager = newSection.GetComponent<SurfTunnelManager>();
                if (manager != null)
                {
                    manager.SetSeed(sectionSeedCounter);
                }

                sectionSeedCounter += 100;
            }
        }
    }
}