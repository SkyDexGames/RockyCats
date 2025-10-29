using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public WaveManager waveManager;
    private bool wavedStarted = false;

    private void Start()
    {
        StartCoroutine(waveManager.StartWaves());
    }

    
}
