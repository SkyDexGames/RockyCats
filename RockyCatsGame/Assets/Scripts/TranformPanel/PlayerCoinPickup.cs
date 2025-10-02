using UnityEngine;

public class PlayerCoinPickup : MonoBehaviour
{
    [SerializeField] CoinInventory inventory;

    private void OnTriggerEnter(Collider other)
    {
        var tag = other.tag;
        if (tag == "Water" || tag == "Heat" || tag == "Pressure" || tag == "Time")
        {
            inventory.AddByTag(tag);
            Destroy(other.gameObject);
        }
    }
}
