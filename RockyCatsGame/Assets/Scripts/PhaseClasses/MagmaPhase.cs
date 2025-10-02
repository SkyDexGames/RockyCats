using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaPhase : PlayerPhase
{
    public float wallSlideMaxFallSpeed = -0.3f;
    public bool IsWallSliding {get; private set;}

    public override void HandleAbility()
    {
        Debug.Log("Magma E ability activated");
    }

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