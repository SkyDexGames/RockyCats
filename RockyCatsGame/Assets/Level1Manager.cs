using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level1Manager : MonoBehaviour
{
    public static Level1Manager Instance;
    
    public int gizmoScore = 0;
    public int chiliScore = 0;
    
    public GameObject gizmoScoreText;
    public GameObject chiliScoreText;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        UpdateScoreDisplays();
    }
    
    public void UpdateScore(string playerName, int scoreToAdd)
    {
        if (playerName.ToLower() == "gizmo")
        {
            gizmoScore += scoreToAdd;
            if (gizmoScore < 0) gizmoScore = 0;
        }
        else if (playerName.ToLower() == "chili")
        {
            chiliScore += scoreToAdd;
            if (chiliScore < 0) chiliScore = 0;
        }
        else
        {
            return;
        }
        
        UpdateScoreDisplays();
    }
    
    public int GetScore(string playerName)
    {
        if (playerName.ToLower() == "gizmo")
            return gizmoScore;
        else if (playerName.ToLower() == "chili")
            return chiliScore;
        else
            return 0;
    }
    
    void UpdateScoreDisplays()
    {
        if (gizmoScoreText != null)
        {
            TextMeshProUGUI gizmoText = gizmoScoreText.GetComponent<TextMeshProUGUI>();
            if (gizmoText != null)
                gizmoText.text = $"Gizmo's Heat Score: {gizmoScore}";
        }
        
        if (chiliScoreText != null)
        {
            TextMeshProUGUI chiliText = chiliScoreText.GetComponent<TextMeshProUGUI>();
            if (chiliText != null)
                chiliText.text = $"Chili's Heat Score: {chiliScore}";
        }
    }
    
    public void ResetScores()
    {
        gizmoScore = 0;
        chiliScore = 0;
        UpdateScoreDisplays();
    }
    
    public void SetScore(string playerName, int newScore)
    {
        if (playerName.ToLower() == "gizmo")
        {
            gizmoScore = newScore;
        }
        else if (playerName.ToLower() == "chili")
        {
            chiliScore = newScore;
        }
        
        UpdateScoreDisplays();
    }
}