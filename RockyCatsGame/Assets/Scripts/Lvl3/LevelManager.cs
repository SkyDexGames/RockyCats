using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;
    public WaveManager waveManager;
    private bool wavedStarted = false;

    public GameObject respawnPoint;

    public GameObject deathPoint;

    public List<PlayerController> alivePlayers;

    private PhotonView photonView;


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
        alivePlayers = new List<PlayerController>();
        photonView = GetComponent<PhotonView>();
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
    
    [PunRPC]
    void PauseRPC()
    {
        Time.timeScale = 0f; 
    }

    
}
