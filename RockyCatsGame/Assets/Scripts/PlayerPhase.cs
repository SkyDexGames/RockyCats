using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Esta clase es para normalizar las fases, es como una especie de template.
*/
public abstract class PlayerPhase : MonoBehaviour
{
    
    /* -1 means no override, si por alguna razon queremos que una fase no overridee un stat, 
     lo dejamos en -1 */
    [SerializeField] protected float moveSpeedOverride = -1f; 
    [SerializeField] protected float jumpForceOverride = -1f;
    [SerializeField] protected float rotationSpeedOverride = -1f;
    
    protected PlayerController playerController;
    
    public virtual void Initialize(PlayerController controller)
    {
        this.playerController = controller;
        ApplyPhaseStats();
    }
    
    public virtual void ApplyPhaseStats()
    {
        // Only override if value is not -1 (default)
        if (moveSpeedOverride >= 0)
            playerController.SetMoveSpeed(moveSpeedOverride);
            
        if (jumpForceOverride >= 0)
            playerController.SetJumpForce(jumpForceOverride);
            
        if (rotationSpeedOverride >= 0)
            playerController.SetRotationSpeed(rotationSpeedOverride);
    }

    public virtual bool IsUsingAbility()
    {
        return false;
    }

    public virtual void HandleMovement(ref Vector3 moveDirection, ref float moveSpeed)
    {
        // Default implementation - does nothing
    }
    
    // Métodos que pueden ser override por fases específicas
    public virtual void HandleAbility() { }
    public virtual void HandleWallSlide(bool isTouchingWall) { }
    public virtual void UpdatePhase() { }
    public virtual void FixedUpdatePhase() { }
}
