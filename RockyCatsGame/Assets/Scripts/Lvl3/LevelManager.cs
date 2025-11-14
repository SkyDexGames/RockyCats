using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.Video;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;
    public WaveManager waveManager;
    private bool wavedStarted = false;

    public GameObject respawnPoint;

    public GameObject deathPoint;

    public List<PlayerController> deathPlayers;



    private PhotonView photonView;

    public TextMeshProUGUI gizmoLifeText;
    public TextMeshProUGUI chiliLifeText;

    public TextMeshProUGUI waveText;

    private int gizmoLife = 100;
    private int chiliLife = 100;

    public VideoPlayer videoPlayer;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        deathPlayers = new List<PlayerController>();
        photonView = GetComponent<PhotonView>();
        UpdateTemperatureDisplays();
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(CheckPlayersDeadRoutine());


    }

    public void LaunchWaves()
    {
        if (PhotonNetwork.IsMasterClient && !wavedStarted)
        {
            wavedStarted = true;
            StartCoroutine(waveManager.StartWaves());
        }
    }

    private void Update()
    {
        
        
    }

    public Vector3 GetSpawnPoint()
    {
        return respawnPoint.transform.position;
    }

    public Vector3 GetDeathPoint()
    {
        return deathPoint.transform.position ;
    }

    public void EndLevel(bool success)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("PauseRPC", RpcTarget.All);
        }
    }

    private IEnumerator CheckPlayersDeadRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            int deadCount = PhotonNetwork.PlayerList.Count(p =>
                p.CustomProperties.ContainsKey("isDead") &&
                (bool)p.CustomProperties["isDead"] == true
            );

            if (deadCount >= PhotonNetwork.PlayerList.Length)
            {
                EndLevel(false);
                yield break;
            }
        }
    }
    public void UpdateMyTemperature(int currentLife)
    {
        bool isMaster = PhotonNetwork.IsMasterClient;
        photonView.RPC("RPC_UpdateTemperature", RpcTarget.All, isMaster, currentLife);
    }
    
    [PunRPC]
    void RPC_UpdateTemperature(bool isMasterClient, int currentLife)
    {
        if (isMasterClient)
        {
            gizmoLife = currentLife;
        }
        else
        {
            chiliLife = currentLife;
        }
        
        UpdateTemperatureDisplays();

        Debug.Log($"Temperature updated - Gizmo: {gizmoLife}, Chili: {chiliLife}");
    }

    void UpdateTemperatureDisplays()
    {
        if (gizmoLifeText != null)
        {
            gizmoLifeText.text = $"Gizmo's Life: {gizmoLife}";
        }

        if (chiliLifeText != null)
        {
            chiliLifeText.text = $"Chili's Life: {chiliLife}";
        }
    }

    public void UpdateWaveDisplay(int currentWave)
    {
        // Update the wave display UI
        waveText.text = $"Wave: {currentWave}";
    }

    [PunRPC]
    void PauseRPC()
    {
        if (videoPlayer != null)
        {
            videoPlayer.enabled = true;
            videoPlayer.Play();

            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.HideAllHUDs();
                HUDManager.Instance.ShowHUD("VideoContainer");
            }
        }
    }

    
}
