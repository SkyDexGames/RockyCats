using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObstacleMovement : MonoBehaviourPun
{
    private float moveSpeed;
    private bool canMove = false;

    [PunRPC]
    void InitializeMovement(float speed)
    {
        Initialize(speed);
    }

    public void Initialize(float speed)
    {
        moveSpeed = speed;
        canMove = true;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && canMove)
        {
            transform.position += new Vector3(0, 0, -moveSpeed) * Time.deltaTime;
        }
    }
}