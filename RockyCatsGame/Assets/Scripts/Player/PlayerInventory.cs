using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;



public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int water;
    [SerializeField] private int temperature;
    [SerializeField] private int pressure;
    [SerializeField] private int time;

    private PhotonView photonView;

    private InventoryUI ui;

    public Button ButtonOpen; // Asignar en el Inspector el botón del Canvas que abre el inventario

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            ui = FindObjectOfType<InventoryUI>();
            ButtonOpen = ui.GetComponentInChildren<Button>();
            if (ButtonOpen != null)
            {
                ButtonOpen.onClick.AddListener(OpenInventory);
            }
            
        }
    }

    public void AddResource(Pickup.PickupType type, int amount)
    {
        if (photonView.IsMine == false) return;
        Debug.Log($"Adding {amount} of {type} to inventory.");
        switch (type)
        {
            case Pickup.PickupType.Water:
                water += amount;
                break;
            case Pickup.PickupType.Temperature:
                temperature += amount;
                break;
            case Pickup.PickupType.Pressure:
                pressure += amount;
                break;
            case Pickup.PickupType.Time:
                time += amount;
                break;
        }
    }

    public int GetResource(Pickup.PickupType type)
    {
        if (photonView.IsMine == false) return -1;

        switch (type)
        {
            case Pickup.PickupType.Water:
                return water;
            case Pickup.PickupType.Temperature:
                return temperature;
            case Pickup.PickupType.Pressure:
                return pressure;
            case Pickup.PickupType.Time:
                return time;
            default:
                return 0;
        }
    }

    public void OpenInventory()
    {
        if (photonView.IsMine == false) return;
        if (ui != null)
        {
            ui.Open(this);
        }
    }


}