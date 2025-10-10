using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    void Awake()
    {
        if(Instance) //if another RoomManager exists
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject); //if im just 1 RoomManager, keep living
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex == 1) //1 is the game scene
        {
            /*PhotonPrefabs must be in the resources folder bc unity automatically excludes any file
            not referenced in the editor from the final build, and we don't reference PhotonPrefabs, we use strings.
            */

            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);

        }
    }
}
