using UnityEngine;



public class Pickup : MonoBehaviour
{
    public enum PickupType { Water, Temperature, Pressure, Time }
    [SerializeField] private PickupType type;
    [SerializeField] private int value;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory player = other.gameObject.GetComponent<PlayerInventory>();
        if (player != null)
        {
            ApplyEffect(player);
            Destroy(gameObject);
        }
    }

    private void ApplyEffect(PlayerInventory player)
    {
        player.AddResource(type, value);
    }
}

