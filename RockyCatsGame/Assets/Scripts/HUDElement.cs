using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDElement : MonoBehaviour
{
    public string hudName;
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }
}