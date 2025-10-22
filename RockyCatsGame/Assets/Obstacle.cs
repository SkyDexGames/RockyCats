using UnityEngine;
using Photon.Pun;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType { Lava, Crystal }
    
    [Header("Obstacle Settings")]
    public ObstacleType obstacleType = ObstacleType.Lava;
    public int scoreChange = 10;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            string playerName = PhotonNetwork.IsMasterClient ? "Gizmo" : "Chili";
            
            int finalScoreChange = obstacleType == ObstacleType.Lava ? scoreChange : -scoreChange;
            
            if (Level1Manager.Instance != null)
            {
                Level1Manager.Instance.UpdateScore(playerName, finalScoreChange);
            }
            
        }
    }
}