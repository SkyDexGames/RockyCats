using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SectionTrigger : MonoBehaviour
{
    private static int sectionSeedCounter = 1000;
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                hasTriggered = true;
                
                bool shouldStopSpawning = false;
                if (Level1Manager.Instance != null)
                {
                    int gizmoTemp = Level1Manager.Instance.GetGizmoTemperature();
                    int chiliTemp = Level1Manager.Instance.GetChiliTemperature();
                    
                    shouldStopSpawning = (gizmoTemp >= 900 && chiliTemp >= 900);
                    
                    Debug.Log($"Checking temperatures - Gizmo: {gizmoTemp}, Chili: {chiliTemp}, Stop Spawning: {shouldStopSpawning}");
                }
                
                if (shouldStopSpawning)
                {
                    Debug.Log("Both players reached 900! Stopping section spawning.");
                    return;
                }
                
                Transform granny = transform.parent.parent;
                GameObject roadSection = Resources.Load<GameObject>("SurfingTunnel");
                
                if (roadSection != null)
                {
                    GameObject newSection = Instantiate(roadSection, granny);
                    newSection.transform.localPosition = new Vector3(-80, 0.7f, 180);

                    SurfTunnelManager manager = newSection.GetComponent<SurfTunnelManager>();
                    if (manager != null)
                    {
                        manager.SetSeed(sectionSeedCounter);
                    }

                    sectionSeedCounter += 100;
                }
                else
                {
                    Debug.LogError("SurfingTunnel prefab not found in Resources folder!");
                }
            }
        }
    }
}