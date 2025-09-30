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
        //spawn pos will be changed later
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), Vector3.zero, Quaternion.identity);
    }
}
