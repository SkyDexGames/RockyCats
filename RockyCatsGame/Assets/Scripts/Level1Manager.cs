using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Level1Manager : MonoBehaviourPun
{
    public static Level1Manager Instance;
    
    private int gizmoTemperature = 0;
    private int chiliTemperature = 0;
    
    public TextMeshProUGUI gizmoTempText;
    public TextMeshProUGUI chiliTempText;
    
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
    
    // Called by the local player when they hit an obstacle
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
        if (gizmoTempText != null)
        {
            gizmoTempText.text = $"Gizmo's Temperature: {gizmoTemperature}°";
        }
        
        if (chiliTempText != null)
        {
            chiliTempText.text = $"Chili's Temperature: {chiliTemperature}°";
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