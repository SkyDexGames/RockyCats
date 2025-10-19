using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;

public class MapManager : MonoBehaviourPunCallbacks
{
    public static MapManager Instance;

    [SerializeField] private Button[] levelButtons;

    [SerializeField] private int[] levelSceneIndexes = { 2, 3, 4, 5, 6, 7, 8, 9 };

    private int selectedLevelIndex = -1; //start with invalid index in case we want to manage a default UI, then add more stuff when we select a lvl

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //setup click listeners
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i;
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
        }
        //SelectLevel(-1);
    }

    void OnLevelButtonClicked(int buttonIndex)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        
        SelectLevel(buttonIndex);
    }

    void SelectLevel(int levelIndex)
    {
        selectedLevelIndex = levelIndex;

        //this is the part where we add visual feedback or sum shit

        Debug.Log($"Selected Level {selectedLevelIndex} -> Scene {levelSceneIndexes[selectedLevelIndex]}");
    }
    
    //instant start (debug)
    void StartLevel()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        int sceneToLoad = levelSceneIndexes[selectedLevelIndex];
        PhotonNetwork.LoadLevel(sceneToLoad);
    }

    public void StartSelectedLevel()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (selectedLevelIndex < 0)
        {
            Debug.Log("Select a valid level first!");
            return;
        }
        PhotonNetwork.LoadLevel(levelSceneIndexes[selectedLevelIndex]);
    }

    //disconnect everyone when someone leaves
    public void LeaveMatch()
    {
        Debug.Log("Leaving match...");
        
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RPC_ForceLeaveRoom", RpcTarget.All);
            }
            else
            {
                //esto puede generar edge cases medio raros hay que checarlo
                PhotonNetwork.LeaveRoom();
            }
        }
        else
        {
            PhotonNetwork.Disconnect();
        }
    }

    [PunRPC]
    void RPC_ForceLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}