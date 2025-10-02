using System;
using UnityEngine;

public enum CoinType { Water, Heat, Pressure, Time }

public class CoinInventory : MonoBehaviour
{
    public int water, heat, pressure, time;
    public event Action OnChanged;

    public int Get(CoinType t) => t switch
    {
        CoinType.Water => water,
        CoinType.Heat => heat,
        CoinType.Pressure => pressure,
        _ => time
    };

    public void Add(CoinType t, int amount = 1)
    {
        switch (t)
        {
            case CoinType.Water: water += amount; break;
            case CoinType.Heat: heat += amount; break;
            case CoinType.Pressure: pressure += amount; break;
            case CoinType.Time: time += amount; break;
        }
        OnChanged?.Invoke();
    }

    public void AddByTag(string tag)
    {
        if (tag == "Water") Add(CoinType.Water);
        else if (tag == "Heat") Add(CoinType.Heat);
        else if (tag == "Pressure") Add(CoinType.Pressure);
        else if (tag == "Time") Add(CoinType.Time);
    }
}
