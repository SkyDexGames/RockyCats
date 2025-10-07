using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;


public class PlayerInventory : MonoBehaviour
{
    public Dictionary<string, int> coins = new Dictionary<string, int>()
    {
        {"water", 0},
        {"temperature", 0},
        {"pressure", 0},
        {"time", 0}

    };

    public Dictionary<string, int> playerStats= new Dictionary<string, int>()
    {
        {"water", 0},
        {"temperature", 5},
        {"pressure", 2},
        {"time", 1}

    };

    private PhotonView photonView;

    public MenuManager menuManager;
    public InventoryUI ui;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        menuManager = FindObjectOfType<MenuManager>();
        if (menuManager != null)
        {
            ui = menuManager.GetComponentInChildren<InventoryUI>(true);
        }

        if (photonView != null && photonView.IsMine)
        {
            if (ui != null)
            {

                ui.setPlayerInventory(this);
            }

        }
    }

    public void AddResource(string type, int amount)
    {
        if (photonView.IsMine == false) return;
        Debug.Log($"Adding {amount} of {type} to inventory.");

        if (coins.ContainsKey(type.ToString().ToLower()))
        {
            coins[type.ToString().ToLower()] += amount;
        }

    }

    public int GetResource(string type)
    {
        if (photonView.IsMine == false) return -1;

        if (coins.ContainsKey(type.ToString().ToLower()))
        {
            return coins[type.ToString().ToLower()];
        }

        return 0;
    }

}