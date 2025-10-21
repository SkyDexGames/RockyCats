using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfTunnelManager : MonoBehaviour
{
    public int gridWidth = 3;
    public int gridLength = 5;
    public float cellSize = 2f;

    //offsets
    public float startXOffset = 0f;
    public float startYOffset = 0f;
    public float startZOffset = 0f;
    public GameObject[] obstaclePrefabs;

    [Range(0f, 1f)] public float spawnChance = 0.3f;

    private List<GameObject> spawnedObstacles = new List<GameObject>();

    void Start()
    {
        SpawnObstacles();
    }

    void Update()
    {
        transform.position += new Vector3(0,0,-2) * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }

    public void SpawnObstacles()
    {
        for (int row = 0; row < gridLength; row++)
        {
            for (int lane = 0; lane < gridWidth; lane++)
            {
                if (Random.value < spawnChance && obstaclePrefabs.Length > 0)
                {
                    Vector3 spawnPos = GetGridPosition(lane, row);
                    GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                    GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity, transform);
                    spawnedObstacles.Add(obstacle);
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
