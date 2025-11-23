using UnityEngine;
using Photon.Pun;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class VideoManager : MonoBehaviourPun
{
    public VideoPlayer videoPlayer;

    private APIRequests apiRequests;

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.enabled = false; // Disable initially
        }
        else
        {
            Debug.LogError("VideoPlayer reference is missing! Drag the VideoPlayer from HUD/VideoContainer/Screen to the inspector.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PhotonNetwork.IsMasterClient && PlayerPrefs.HasKey("PlayerUsername"))
            {
                if(PlayerPrefs.GetInt("PlayerLevels") < 1)
                {
                    
                    Debug.Log("[Level2Manager] Actualizando niveles del jugador en el servidor...");
                    apiRequests = new APIRequests();
                    string username = PlayerPrefs.GetString("PlayerUsername");
                    StartCoroutine(apiRequests.UpdatePlayerLevels(username, 1,
                        onSuccess: () => {
                            Debug.Log("Niveles del jugador actualizados correctamente.");
                            PlayerPrefs.SetInt("PlayerLevels", 1);
                        },
                        onError: (error) => {
                            Debug.LogError("Error al actualizar niveles del jugador: " + error);
                        }
                    ));
                }
            }
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                photonView.RPC("RPC_PlayVideoForAll", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void RPC_PlayVideoForAll()
    {
        if (videoPlayer != null)
        {
            videoPlayer.enabled = true;
            videoPlayer.Play();

            if (Level1Manager.Instance != null)
            {
                Level1Manager.Instance.ShowHUD("VideoContainer");
            }
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        photonView.RPC("RPC_LoadScene", RpcTarget.All, 1);
    }

    [PunRPC]
    void RPC_LoadScene(int sceneIndex)
    {
        PhotonNetwork.LoadLevel(sceneIndex);
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}