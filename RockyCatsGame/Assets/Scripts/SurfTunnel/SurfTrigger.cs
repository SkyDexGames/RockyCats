using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SurfTrigger : MonoBehaviour
{
    [SerializeField] private SurfTunnelManager firstPlatform;
    private List<int> playersInTrigger = new List<int>();
    private bool triggered = false;
    private PhotonView pv;

    void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.SetToHalted();

                    Animator currentAnimator = playerController.GetCurrentAnimator();
                    currentAnimator.SetBool("pullBoard", true);

                    // Play pullBoard animation sound
                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlayLevelSFX("PullBoard");
                    }

                    if (pv != null)
                    {
                        pv.RPC("RPC_AddPlayerToTrigger", RpcTarget.AllBuffered, playerPhotonView.ViewID);
                    }
                }
            }
        }
    }

    [PunRPC]
    void RPC_AddPlayerToTrigger(int playerViewID)
    {
        if (!playersInTrigger.Contains(playerViewID))
        {
            playersInTrigger.Add(playerViewID);
            
            CheckAllPlayersReady();
        }
    }

    void CheckAllPlayersReady()
    {
        if (playersInTrigger.Count >= PhotonNetwork.PlayerList.Length)
        {
            triggered = true;
            //StartSurfingMode();
            if (pv != null)
            {
                pv.RPC("RPC_StartSurfingWithDelay", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void RPC_StartSurfingWithDelay()
    {
        StartCoroutine(StartSurfingAfterDelay());
    }


    IEnumerator StartSurfingAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        StartSurfingMode();
    }
    
    void StartSurfingMode()
    {
        Level1Manager.Instance.ShowHUD("SurfScores");

        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in allPlayers)
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                player.SetToSurfing();
            }
        }
        
        if (firstPlatform != null)
        {
            SurfTunnelManager tunnelManager = firstPlatform.GetComponent<SurfTunnelManager>();
            if (tunnelManager != null)
            {
                tunnelManager.StartMoving();
            }
        }
        
        Destroy(gameObject);
    }

    [PunRPC]
    void RPC_SetAllPlayersSurfing()
    {
        
    }
}