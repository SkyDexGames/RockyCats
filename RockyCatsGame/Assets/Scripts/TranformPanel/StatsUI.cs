using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField] Stats stats;

    [Header("Texts (números)")]
    [SerializeField] TextMeshProUGUI waterText;
    [SerializeField] TextMeshProUGUI heatText;
    [SerializeField] TextMeshProUGUI pressureText;
    [SerializeField] TextMeshProUGUI timeText;

    [Header("Barras (opcional)")]
    [SerializeField] Image[] waterBars;
    [SerializeField] Image[] heatBars;
    [SerializeField] Image[] pressureBars;
    [SerializeField] Image[] timeBars;

    void OnEnable()
    {
        if (stats != null) stats.OnChanged += Refresh;
        Refresh();
    }
    void OnDisable()
    {
        if (stats != null) stats.OnChanged -= Refresh;
    }

    public void Refresh()
    {
        if (stats == null) return;

        waterText.text = stats.water.ToString();
        heatText.text = stats.heat.ToString();
        pressureText.text = stats.pressure.ToString();
        timeText.text = stats.time.ToString();

        UpdateBars(waterBars, stats.water);
        UpdateBars(heatBars, stats.heat);
        UpdateBars(pressureBars, stats.pressure);
        UpdateBars(timeBars, stats.time);
    }

    void UpdateBars(Image[] bars, int value)
    {
        if (bars == null) return;
        for (int i = 0; i < bars.Length; i++)
            bars[i].enabled = i < value; // cuadraditos llenos/vacíos
    }
}
