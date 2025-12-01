using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Video;
using Photon.Realtime;

public class MapManager : MonoBehaviourPunCallbacks
{
    public static MapManager Instance;

    [SerializeField] private Button[] levelButtons;

    [SerializeField] private HUDElement[] hudElements;

    [SerializeField] private int[] levelSceneIndexes = { 2, 3, 4, 5, 6, 7, 8, 9 };

    private int selectedLevelIndex = -1; //start with invalid index in case we want to manage a default UI, then add more stuff when we select a lvl

    public VideoPlayer VideoPlayer;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //setup click listeners
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_PlayIntro", RpcTarget.All);
        }
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i;
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClicked(levelIndex));
        }
        //SelectLevel(-1);
        
        for (int i=0; i<levelButtons.Length; i++)
        {
            //CAMBIAR EL TRES DESPUES
            if (i <= PlayerPrefs.GetInt("PlayerLevels", 2))
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }

    }

    [PunRPC]
    void RPC_PlayIntro()
    {
        if (PlayerPrefs.HasKey("PlayIntroCutscene"))
        {
            VideoPlayer.Stop();
            HUDManager.Instance.HideHUD("VideoContainer");

            MenuManager.Instance.OpenMenu("Map");

            HUDManager.Instance.HideHUD("Background");
        }
        else{
        AudioManager.Instance.StopBGM();
        HUDManager.Instance.ShowHUD("VideoContainer");

        VideoPlayer.loopPointReached -= OnVideoFinished;
        VideoPlayer.loopPointReached += OnVideoFinished;

        VideoPlayer.enabled = true;
        string videoPath = Application.streamingAssetsPath + "/Introscene1.MP4";
        VideoPlayer.url = videoPath;
        VideoPlayer.Play();
        PlayerPrefs.SetInt("PlayIntroCutscene", 1);
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("El video terminó");

        VideoPlayer.Stop();
        AudioManager.Instance.ResumeLevelBGM();
        HUDManager.Instance.HideHUD("VideoContainer");

        MenuManager.Instance.OpenMenu("Map");

        HUDManager.Instance.HideHUD("Background");

    }

    void OnLevelButtonClicked(int buttonIndex)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        SelectLevel(buttonIndex);
        Debug.Log("selected");
        ShowHUD("StartLevelButton");

        photonView.RPC("RPC_UpdateCatIcons", RpcTarget.All, buttonIndex);
    }

    [PunRPC]
    void RPC_UpdateCatIcons(int levelIndex)
    {
        UpdateCatIcons(levelIndex);
    }

    void UpdateCatIcons(int levelIndex)
    {
        HideHUD("CatIconsLvl1");
        HideHUD("CatIconsLvl2");
        HideHUD("CatIconsLvl3");
        
        ShowHUD($"CatIconsLvl{levelIndex + 1}");
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
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void ShowHUD(string hudName)
    {
        for (int i = 0; i < hudElements.Length; i++)
        {
            if (hudElements[i].hudName == hudName)
            {
                hudElements[i].Show();
                return;
            }
        }
    }
    public void HideHUD(string hudName)
    {
        for (int i = 0; i < hudElements.Length; i++)
        {
            if (hudElements[i].hudName == hudName)
            {
                hudElements[i].Hide();
                return;
            }
        }
    }

}