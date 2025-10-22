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

    //offsets
    public float startXOffset = 0f;
    public float startYOffset = 0f;
    public float startZOffset = 0f;

    public string[] obstaclePrefabPaths = {
        "PhotonPrefabs/Level1/BADCUBE",
        "PhotonPrefabs/Level1/GOODCUBE"
    };

    [Range(0f, 1f)] public float spawnChance = 0.3f;

    private List<GameObject> spawnedObstacles = new List<GameObject>();

    private bool canMove = false;


    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnObstacles();
            
            if (!isFirstPlatform)
            {
                canMove = true;
            }
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && canMove)
        {
            transform.position += new Vector3(0, 0, -20) * Time.deltaTime;
        }
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
        Debug.Log("SurfTunnelManager: StartMoving called");
        canMove = true;
    }

    public void SpawnObstacles()
    {
        if (!spawnsObstacles) return;

        for (int row = 0; row < gridLength; row++)
        {
            for (int lane = 0; lane < gridWidth; lane++)
            {
                if (Random.value < spawnChance && obstaclePrefabPaths.Length > 0)
                {
                    Vector3 spawnPos = GetGridPosition(lane, row);
                    string obstaclePath = obstaclePrefabPaths[Random.Range(0, obstaclePrefabPaths.Length)];
                    GameObject obstacle = PhotonNetwork.Instantiate(obstaclePath, spawnPos, Quaternion.identity);obstacle.transform.SetParent(transform);
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
