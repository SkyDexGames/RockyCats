using UnityEngine;



public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int water;
    [SerializeField] private int temperature;
    [SerializeField] private int pressure;
    [SerializeField] private int time;

    public void AddResource(Pickup.PickupType type, int amount)
    {
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
}