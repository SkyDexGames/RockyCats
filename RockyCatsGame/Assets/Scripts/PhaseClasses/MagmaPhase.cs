using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaPhase : PlayerPhase
{
    public float wallSlideGravityMultiplier = 0.3f;
    public bool IsWallSliding {get; private set;}
    private float originalMoveSpeed;

    public override void HandleAbility(){
        Debug.Log("Magma E ability activated");
    }

    public override void HandleWallSlide(bool isTouchingWall){
        bool wasWallSliding = IsWallSliding;
        IsWallSliding = isTouchingWall && !playerController.IsGrounded && playerController.GetVerticalVelocity() < 0;

        if(IsWallSliding && !wasWallSliding)
        {
            originalMoveSpeed = playerController.GetMoveSpeed;
            playerController.SetMoveSpeed(originalMoveSpeed * 0.1f);
        }
        else if(!IsWallSliding && wasWallSliding)
        {
            playerController.SetMoveSpeed(originalMoveSpeed);
        }
    }
}