using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    /*
    PlayerManager should manage:
    1. Persistent player data
    2. Respawns and death
    3. Receiving info from other players
    */
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(PV.IsMine){
            CreateController();
        }
        
    }

    void CreateController()
    {
        //set spawn pos based on host/non-host
        Vector3 spawnPosition = GetSpawnPosition();
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPosition, Quaternion.identity);
    }

    Vector3 GetSpawnPosition()
    {
        bool isHost = PhotonNetwork.IsMasterClient;
        
        if (isHost)
        {
            return new Vector3(-10, 0.5f, 0);
        }
        else
        {
            return new Vector3(-5, 0.5f, 0);
        }
    }
}
