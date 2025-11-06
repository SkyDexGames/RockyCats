using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class WaveManager : MonoBehaviourPun
{
    public static WaveManager Instance;

    public int totalWaves = 10;
    public float delayBetweenWaves = 3f;

    public int CurrentWave = 0;

    private bool waveEnded = false;

    public List<AttackContainer> AttackContainers;
    public List<RadialShotPattern> patternsList;

    public GameObject windBlower;



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
            int containerIndex = Random.Range(0, AttackContainers.Count);
            int patternIndex = Random.Range(0, patternsList.Count);
            bool shootWind = Random.value > 0.5f;

            // Enviar la instrucción a TODOS los jugadores
            photonView.RPC("RPC_ExecuteContainerPattern", RpcTarget.All,
                containerIndex, patternIndex, shootWind);

            yield return new WaitForSeconds(delayBetweenContainers);
        }
    }

    [PunRPC]
    private void RPC_ExecuteContainerPattern(int containerIndex, int patternIndex, bool shootWind)
    {
        

        AttackContainer container = AttackContainers[containerIndex];
        RadialShotPattern pattern = patternsList[patternIndex];

        StartCoroutine(container.ExecutePattern(pattern));

        if (shootWind)
            windBlower.SetActive(true);
    }

}
