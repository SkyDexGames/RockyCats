using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MagmaPhase : PlayerPhase
{
    public float wallSlideMaxFallSpeed = -0.1f;
    public bool IsWallSliding {get; private set;}
    private float heatTransferRate = 5f; //heat per second

    private bool isHeating;

    public override void HandleAbility()
    {
        Debug.Log("Magma E ability activated");
        isHeating = true;
        playerController.SetUsingAbility(true);
    }

    public override void UpdatePhase() //es como el update pero aplicado para nuestras fases
    {
        base.UpdatePhase();

        if(isHeating)
        {
            //hacemos magia con physics porque detectar colisiones no jala cuando no estamos trabajando directamente en el PlayerController
            Collider[] nearby = Physics.OverlapSphere(
                playerController.transform.position, 
                0.2f
            );

            foreach(Collider col in nearby)
            {
                Heatable heatable = col.GetComponent<Heatable>();
                if(heatable != null)
                {
                    float heatThisFrame = heatTransferRate * Time.deltaTime;
                    heatable.ReceiveHeat(heatThisFrame);
                }
            }
        }

        //keep heating while E is held down
        if(isHeating && !Input.GetKey(KeyCode.E))
        {
            isHeating = false;
            playerController.SetUsingAbility(false);
            Debug.Log("Heating stopped");
        }
    }
    /*
    void OnCollisionStay(Collision collision)
    {
        if(!isHeating) return;

        Heatable heatable = collision.gameObject.GetComponent<Heatable>();
        
        if(heatable != null)
        {
            float heatThisFrame = heatTransferRate * Time.deltaTime;
            heatable.ReceiveHeat(heatThisFrame);
        }
    }*/

    public override void HandleWallSlide(bool isTouchingWall)
    {
        IsWallSliding = isTouchingWall && !playerController.IsGrounded && playerController.GetVerticalVelocity() < 0;

        if(IsWallSliding)
        {
            if(playerController.GetVerticalVelocity() < wallSlideMaxFallSpeed)
            {
                playerController.SetVerticalVelocity(wallSlideMaxFallSpeed);
            }
        }
    }
}