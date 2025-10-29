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

        int numberOfContainers = CurrentWave * 2;  // Por ejemplo, más contenedores en cada wave
        float delayBetweenContainers = 1.5f;

        //Logica de la waves
        for (int i = 0; i < numberOfContainers; i++)
        {
            AttackContainer container = AttackContainers[Random.Range(0, AttackContainers.Count)];
            RadialShotPattern pattern = patternsList[Random.Range(0, patternsList.Count)];

            StartCoroutine(container.ExecutePattern(pattern));

            if (Random.value > 0.5f)
            {
                container.ShootWind();
            }
            yield return new WaitForSeconds(delayBetweenContainers);
        }
    }

}
