using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;


public partial class InventoryUI : MonoBehaviour
{
    
    public void SubtractStat(string statName)
    {
        if (currentInventory == null) return;

        int previousDistance = Mathf.Abs(temporalStats[statName] - initialStats[statName]);

        temporalStats[statName]--;

        // Evita valores negativos (opcional)
        if (temporalStats[statName] < 0)
        {
            temporalStats[statName] = 0;
            return;
        }

        int newDistance = Mathf.Abs(temporalStats[statName] - initialStats[statName]);

        if (newDistance > previousDistance)
        {
            // te alejaste → cuesta moneda
            if (temporalCoins[statName] > 0)
                temporalCoins[statName]--;
            else
                Debug.LogWarning($"No hay monedas de {statName} para bajar más.");
        }
        else if (newDistance < previousDistance)
        {
            // te acercaste → recuperas moneda
            temporalCoins[statName]++;
        }

        UpdateCoinTexts();
        Debug.Log($"{statName} ↓ → stat={temporalStats[statName]} | coins={temporalCoins[statName]}");
    }

    public void AddStat(string statName)
    {
        if (currentInventory == null) return;

        int previousDistance = Mathf.Abs(temporalStats[statName] - initialStats[statName]);

        // Solo sube si tienes monedas
        if (temporalCoins[statName] <= 0)
        {
            Debug.LogWarning($"No hay monedas disponibles de {statName}");
            return;
        }

        temporalStats[statName]++;

        int newDistance = Mathf.Abs(temporalStats[statName] - initialStats[statName]);

        // Si se aleja, gasta moneda. Si se acerca, recupera.
        if (newDistance > previousDistance)
            temporalCoins[statName]--;
        else if (newDistance < previousDistance)
            temporalCoins[statName]++;

        UpdateCoinTexts();
        Debug.Log($"{statName} ↑ → stat={temporalStats[statName]} | coins={temporalCoins[statName]}");
    }

    



}