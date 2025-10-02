using TMPro;
using UnityEngine;

public class InventoryHUD : MonoBehaviour
{
    [SerializeField] CoinInventory inventory;
    [SerializeField] TextMeshProUGUI waterText;
    [SerializeField] TextMeshProUGUI heatText;
    [SerializeField] TextMeshProUGUI pressureText;
    [SerializeField] TextMeshProUGUI timeText;

    void OnEnable() { if (inventory) inventory.OnChanged += Refresh; Refresh(); }
    void OnDisable() { if (inventory) inventory.OnChanged -= Refresh; }

    public void Refresh()
    {
        if (!inventory) return;
        waterText.text = inventory.water.ToString();
        heatText.text = inventory.heat.ToString();
        pressureText.text = inventory.pressure.ToString();
        timeText.text = inventory.time.ToString();
    }
}
