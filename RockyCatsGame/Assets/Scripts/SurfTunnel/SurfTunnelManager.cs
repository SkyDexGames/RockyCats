using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SurfTunnelManager : MonoBehaviour
{
    
    public bool spawnsObstacles = true;
    public bool isFirstPlatform = false;
    
    public int gridWidth = 3;
    public int gridLength = 5;
    public float cellSize = 2f;

    public float startXOffset = 0f;
    public float startYOffset = 0f;
    public float startZOffset = 0f;

    public GameObject[] obstaclePrefabs;

    [Range(0f, 1f)] public float spawnChance = 0.3f;

    private bool canMove = false;
    private int seed;

    private static int baseSeed = -1;
    private static int seedCounter = 0;

    void Start()
    {
        if (isFirstPlatform && baseSeed == -1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                baseSeed = Random.Range(0, 1000000);
                
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
                {
                    { "BaseSeed", baseSeed }
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
            else
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BaseSeed"))
                {
                    baseSeed = (int)PhotonNetwork.CurrentRoom.CustomProperties["BaseSeed"];
                }
                else
                {
                    Debug.LogWarning("BaseSeed not found in room properties! Waiting...");
                    StartCoroutine(WaitForBaseSeed());
                    return;
                }
            }
            
            seedCounter = 0;
        }
        
        seed = baseSeed + seedCounter;
        seedCounter++;

        if (spawnsObstacles)
        {
            SpawnObstacles();
        }

        if (!isFirstPlatform)
        {
            canMove = true;
        }
    }

    IEnumerator WaitForBaseSeed()
    {
        while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("BaseSeed"))
        {
            yield return new WaitForSeconds(0.1f);
        }
        
        baseSeed = (int)PhotonNetwork.CurrentRoom.CustomProperties["BaseSeed"];
        seedCounter = 0;
        
        seed = baseSeed + seedCounter;
        seedCounter++;

        if (spawnsObstacles)
        {
            SpawnObstacles();
        }

        if (!isFirstPlatform)
        {
            canMove = true;
        }
    }

    void Update()
    {
        if (canMove)
        {
            transform.position += new Vector3(0, 0, -20) * Time.deltaTime;
        }
        Debug.Log($"Current Seed: {seed}");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }

    public void StartMoving()
    {
        canMove = true;
    }

    public void SpawnObstacles()
    {
        if (!spawnsObstacles) return;

        System.Random random = new System.Random(seed);

        for (int row = 0; row < gridLength; row++)
        {
            for (int lane = 0; lane < gridWidth; lane++)
            {
                if (random.NextDouble() < spawnChance && obstaclePrefabs.Length > 0)
                {
                    Vector3 spawnPos = GetGridPosition(lane, row);
                    GameObject obstaclePrefab = obstaclePrefabs[random.Next(0, obstaclePrefabs.Length)];
                    GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, obstaclePrefab.transform.rotation);
                    obstacle.transform.SetParent(transform);
                }
            }
        }
    }

    Vector3 GetGridPosition(int lane, int row)
    {
        float xPos = (lane - (gridWidth - 1) / 2f) * cellSize + startXOffset;
        float yPos = startYOffset;
        float zPos = row * cellSize + startZOffset;

        return transform.position + new Vector3(xPos, yPos, zPos);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        for (int row = 0; row < gridLength; row++)
        {
            for (int lane = 0; lane < gridWidth; lane++)
            {
                Vector3 pos = GetGridPosition(lane, row);
                Gizmos.DrawWireCube(pos, Vector3.one * (cellSize * 0.8f));
            }
        }
    }
}