using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.UI;

public class Level1Manager : MonoBehaviourPun
{
    public static Level1Manager Instance;
    
    private int gizmoTemperature = 0;
    private int chiliTemperature = 0;

    public Image gizmosTempBar;
    public Image chilisTempBar;
    

    [SerializeField] private HUDElement[] hudElements;
    private bool isPaused = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        UpdateTemperatureDisplays();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
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
    
    public void ShowHUDs(params string[] hudNames)
    {
        foreach (string name in hudNames)
        {
            ShowHUD(name);
        }
    }

    public void HideHUDs(params string[] hudNames)
    {
        foreach (string name in hudNames)
        {
            HideHUD(name);
        }
    }
    
    public void ShowAllHUDs()
    {
        for (int i = 0; i < hudElements.Length; i++)
        {
            hudElements[i].Show();
        }
    }

    public void HideAllHUDs()
    {
        for (int i = 0; i < hudElements.Length; i++)
        {
            hudElements[i].Hide();
        }
    }
    
    public void EnableSurfMode()
    {
        ShowHUD("Scores");
    }

    public void DisableSurfMode()
    {
        HideHUD("Scores");
    }

    public void TogglePause()
    {
        isPaused = !isPaused; 
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
        
    }

    public void PauseGame()
    {
        ShowHUD("PauseMenu");
        //Time.timeScale = 0f;
        //set player mode to halted
    }
    
    public void ResumeGame()
    {
        HideHUD("PauseMenu");
        //Time.timeScale = 1f;
        //set player mode to normal
    }

    public void UpdateMyTemperature(int tempChange)
    {
        // Determine if this client is master or not
        bool isMaster = PhotonNetwork.IsMasterClient;
        
        // Send RPC to all clients to update the score
        photonView.RPC("RPC_UpdateTemperature", RpcTarget.All, isMaster, tempChange);
    }
    
    [PunRPC]
    void RPC_UpdateTemperature(bool isMasterClient, int tempChange)
    {
        if (isMasterClient)
        {
            gizmoTemperature = Mathf.Max(0, gizmoTemperature + tempChange);
        }
        else
        {
            chiliTemperature = Mathf.Max(0, chiliTemperature + tempChange);
        }
        
        UpdateTemperatureDisplays();
        
        Debug.Log($"Temperature updated - Gizmo: {gizmoTemperature}, Chili: {chiliTemperature}");
    }
    
    void UpdateTemperatureDisplays()
    {
        if (gizmosTempBar != null)
        {
            gizmosTempBar.fillAmount = gizmoTemperature / 900f;

        }
        
        if (chilisTempBar != null)
        {
            chilisTempBar.fillAmount = chiliTemperature / 900f;
        }
    }
    
    public int GetGizmoTemperature()
    {
        return gizmoTemperature;
    }
    
    public int GetChiliTemperature()
    {
        return chiliTemperature;
    }
}