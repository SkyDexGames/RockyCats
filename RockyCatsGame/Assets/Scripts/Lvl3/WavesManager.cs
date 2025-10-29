using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public int totalWaves = 10;
    public float delayBetweenWaves = 3f;

    public int CurrentWave = 0;

    private bool waveEnded = false;

    public List<AttackContainer> AttackContainers;
    public List<RadialShotPattern> patternsList;



    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator StartWaves()
    {

       for (int i = 0; i < totalWaves; i++)
        {
            yield return StartCoroutine(StartNextWave());
            yield return new WaitForSeconds(delayBetweenWaves);
        }

        Debug.Log(" Todas las waves completadas.");
    }


    public IEnumerator StartNextWave()
    {
        if (CurrentWave >= 10)
        {
            Debug.Log("All waves completed");
            yield break;
        }
        CurrentWave++;
        Debug.Log($"Starting Wave {CurrentWave}");


        for (int i = 0; i < CurrentWave * 2; i++)
        {
            AttackContainer container = AttackContainers[Random.Range(0, AttackContainers.Count)];
            RadialShotPattern pattern = patternsList[Random.Range(0, patternsList.Count)];

            yield return StartCoroutine(container.ExecutePattern(pattern));
        }
    }

}
