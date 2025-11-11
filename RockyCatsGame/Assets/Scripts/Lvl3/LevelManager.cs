using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour
{
    public WaveManager waveManager;
    private bool wavedStarted = false;

    public GameObject respawnPoint;

    private void Start()
    {
        
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

    
}
