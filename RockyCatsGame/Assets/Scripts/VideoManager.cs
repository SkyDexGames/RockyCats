using UnityEngine;
using Photon.Pun;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoManager : MonoBehaviourPun
{
    public VideoPlayer videoPlayer;

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