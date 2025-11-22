using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    /*
    i fucked up the level1manager singleton my giving it too many responsibilities
    so here im making a new, single purpose singleton which will just manage the hud.
    The level1manager will be changed in the future so this is used as UI manager instead.
    */
    public static HUDManager Instance {get; private set;}

    [SerializeField] private HUDElement[] hudElements;

    private bool isPaused = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void ShowHUD(string hudName)
    {
        for (int i = 0; i < hudElements.Length; i++)
        {
            if (hudElements[i].hudName == hudName)
            {
                hudElements[i].Show();
                return;
            }
        }
    }

    public void HideHUD(string hudName)
    {
        for (int i = 0; i < hudElements.Length; i++)
        {
            if (hudElements[i].hudName == hudName)
            {
                hudElements[i].Hide();
                return;
            }
        }
    }
    
    public void ShowHUDs(params string[] hudNames)
    {
        foreach (string name in hudNames)
        {
            ShowHUD(name);
        }
    }

    public void HideHUDs(params string[] hudNames)
    {
        foreach (string name in hudNames)
        {
            HideHUD(name);
        }
    }
    
    public void ShowAllHUDs()
    {
        for (int i = 0; i < hudElements.Length; i++)
        {
            hudElements[i].Show();
        }
    }

    public void HideAllHUDs()
    {
        for (int i = 0; i < hudElements.Length; i++)
        {
            hudElements[i].Hide();
        }
    }
    
    public void EnableSurfMode()
    {
        ShowHUD("Scores");
    }

    public void DisableSurfMode()
    {
        HideHUD("Scores");
    }

    public void TogglePause()
    {
        isPaused = !isPaused; 
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
        
    }

    public void PauseGame()
    {
        ShowHUD("PauseMenu");
        //Time.timeScale = 0f;
        //set player mode to halted
    }
    
    public void ResumeGame()
    {
        HideHUD("PauseMenu");
        //Time.timeScale = 1f;
        //set player mode to normal
    }
    public void QuitGame()
    {
        Application.Quit();
    }


}
