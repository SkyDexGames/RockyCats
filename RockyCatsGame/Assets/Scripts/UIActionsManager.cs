using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActionsManager : MonoBehaviour
{

    public static UIActionsManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void OpenTransformsMenu()
    {
        
        if (MenuManager.Instance == null)
        {
            Debug.LogError("MenuManager Instance is NULL!");
            return;
        }
        
        MenuManager.Instance.OpenMenu("TransformsMenu");
    }

    public void OpenHUD()
    {
        
        if (MenuManager.Instance == null)
        {
            Debug.LogError("MenuManager Instance is NULL!");
            return;
        }
        
        MenuManager.Instance.OpenMenu("HUD");
    }
}
