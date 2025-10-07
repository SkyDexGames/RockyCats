using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [SerializeField] public GameObject WaterItemText;
    [SerializeField] public GameObject TempItemText;
    [SerializeField] public GameObject PressureItemText;
    [SerializeField] public GameObject TimeItemText;

    private PlayerInventory currentInventory;


    void Awake()
    {
        panel.SetActive(false);
    }
    public void Open(PlayerInventory inventory)
    {
        currentInventory = inventory;
        setValues();
    }

    public void Close()
    {
        panel.SetActive(false);
        currentInventory = null;
    }

    public void setValues()
    {
        if (currentInventory == null) return;

        int water = currentInventory.GetResource(Pickup.PickupType.Water);
        int temp = currentInventory.GetResource(Pickup.PickupType.Temperature);
        int pressure = currentInventory.GetResource(Pickup.PickupType.Pressure);
        int time = currentInventory.GetResource(Pickup.PickupType.Time);

        WaterItemText.GetComponent<Text>().text = water.ToString();
        TempItemText.GetComponent<Text>().text = temp.ToString();
        PressureItemText.GetComponent<Text>().text = pressure.ToString();
        TimeItemText.GetComponent<Text>().text = time.ToString();
    }



}
