using UnityEngine;
using System;

public enum StatType { Water, Heat, Pressure, Time }

public class Stats : MonoBehaviour
{
    public const int MAX = 5; 

    [Range(0, 5)] public int water;
    [Range(0, 5)] public int heat;
    [Range(0, 5)] public int pressure;
    [Range(0, 5)] public int time;

    public event Action OnChanged;

    public void Add(StatType t, int amount = 1)
    {
        switch (t)
        {
            case StatType.Water: water = Mathf.Clamp(water + amount, 0, MAX); break;
            case StatType.Heat: heat = Mathf.Clamp(heat + amount, 0, MAX); break;
            case StatType.Pressure: pressure = Mathf.Clamp(pressure + amount, 0, MAX); break;
            case StatType.Time: time = Mathf.Clamp(time + amount, 0, MAX); break;
        }
        OnChanged?.Invoke();
    }

    public void AddByTag(string tag)
    {
        switch (tag)
        {
            case "Water": Add(StatType.Water); break;
            case "Heat": Add(StatType.Heat); break;
            case "Pressure": Add(StatType.Pressure); break;
            case "Time": Add(StatType.Time); break;
        }
    }
}
