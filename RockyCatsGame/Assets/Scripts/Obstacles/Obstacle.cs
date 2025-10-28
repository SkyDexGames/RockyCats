using UnityEngine;
using Photon.Pun;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType { Magma, Crystal }
    
    public ObstacleType obstacleType = ObstacleType.Magma;
    public int temperatureChange = 10;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();
            
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                int finalTempChange = obstacleType == ObstacleType.Magma ? temperatureChange : -temperatureChange;
                
                if (Level1Manager.Instance != null)
                {
                    Level1Manager.Instance.UpdateMyTemperature(finalTempChange);
                }
                
                //Destroy(gameObject);
            }
        }
    }
}