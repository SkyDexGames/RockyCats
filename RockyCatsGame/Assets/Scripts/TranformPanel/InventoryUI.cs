using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public partial class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [SerializeField] public GameObject WaterItemText;
    [SerializeField] public GameObject TempItemText;
    [SerializeField] public GameObject PressureItemText;
    [SerializeField] public GameObject TimeItemText;

    public Dictionary<string, int> temporalCoins = new Dictionary<string, int>()
    {
        {"water", 0},
        {"temperature", 0},
        {"pressure", 0},
        {"time", 0}
    };

    public Dictionary<string, int> initialCoins = new Dictionary<string, int>()
    {
        {"water", 0},
        {"temperature", 0},
        {"pressure", 0},
        {"time", 0}
    };

    public Dictionary<string, int> temporalStats = new Dictionary<string, int>()
    {
        {"water", 0},
        {"temperature", 0},
        {"pressure", 0},
        {"time", 0}
    };

    public Dictionary<string, int> initialStats = new Dictionary<string, int>()
    {
        {"water", 0},
        {"temperature", 0},
        {"pressure", 0},
        {"time", 0}
    };

    public Dictionary<string, int> currentStats;


    private PlayerInventory currentInventory;


    void Awake()
    {
        
        panel.SetActive(false);

    }
    public void setPlayerInventory(PlayerInventory inventory)
    {
        currentInventory = inventory;
        setValues();
    }


    public void setValues()
    {
        if (currentInventory == null) return;

        string[] states = { "water", "temperature", "pressure", "time" };
        foreach (string state in states)
        {
            temporalCoins[state] = currentInventory.GetResource(state);
            initialCoins[state] = temporalCoins[state];
            temporalStats[state] = currentInventory.playerStats[state];
            initialStats[state] = temporalStats[state];
        }

        UpdateCoinTexts();

    }

    private void UpdateCoinTexts()
    {
        WaterItemText.GetComponent<TextMeshProUGUI>().text = temporalCoins["water"].ToString();
        TempItemText.GetComponent<TextMeshProUGUI>().text = temporalCoins["temperature"].ToString();
        PressureItemText.GetComponent<TextMeshProUGUI>().text = temporalCoins["pressure"].ToString();
        TimeItemText.GetComponent<TextMeshProUGUI>().text = temporalCoins["time"].ToString();
    }

    void OnEnable()
    {

        setValues();
        
    }



}
