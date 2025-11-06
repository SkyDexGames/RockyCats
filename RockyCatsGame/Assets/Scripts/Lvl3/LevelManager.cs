using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance;
    public WaveManager waveManager;
    private bool wavedStarted = false;

    public GameObject respawnPoint;

    public GameObject deathPoint;

    public List<PlayerController> deathPlayers;



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
        deathPlayers = new List<PlayerController>();
        photonView = GetComponent<PhotonView>();
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
        
    
    
    [PunRPC]
    void PauseRPC()
    {
        Time.timeScale = 0f; 
    }

    
}
