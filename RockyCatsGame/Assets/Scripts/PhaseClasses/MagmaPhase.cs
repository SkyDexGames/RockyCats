using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MagmaPhase : PlayerPhase
{
    public float wallSlideMaxFallSpeed = -0.1f;
    public bool IsWallSliding {get; private set;}

    //heat charge
    [SerializeField] private float maxCharge = 100f;
    [SerializeField] private float chargeRate = 10f; //charge per second
    //[SerializeField] private float releaseHeatTransfer = 50f;

    private float currentCharge = 0f;
    private bool isCharging = false;
    private bool canRelease = false;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public override void HandleAbility()
    {
        if (!photonView.IsMine) return;
        
        Debug.Log("Magma E ability - Start Charging");
        isCharging = true;
        canRelease = false;
        playerController.SetUsingAbility(true);
    }

    public override void UpdatePhase()
    {
        base.UpdatePhase();

        if (!photonView.IsMine) return;

        HandleCharging();
        HandleRelease();
    }

    private void HandleCharging()
    {
        if (isCharging && Input.GetKey(KeyCode.E))
        {
            //cargar mientras holdeas E
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxCharge);
            
            if (currentCharge >= 10f) //carga minima para darle release
            {
                canRelease = true;
            }
            
            Debug.Log($"Charging: {currentCharge}/{maxCharge}");
        }
    }

    private void HandleRelease()
    {
        //soltar carga si tienes mas de lo necesario Y sueltas E
        if (isCharging && !Input.GetKey(KeyCode.E) && canRelease)
        {
            ReleaseHeat();
        }
        // cancelar si tienes menos y sueltas E
        else if (isCharging && !Input.GetKey(KeyCode.E) && !canRelease)
        {
            CancelCharge();
        }
    }

    private void ReleaseHeat()
    {
        // Find nearby heatable objects and apply heat
        Collider[] nearby = Physics.OverlapSphere(playerController.transform.position, 0.5f);
        bool hitObject = false;

        foreach(Collider col in nearby)
        {
            Heatable heatable = col.GetComponent<Heatable>();
            if(heatable != null)
            {
                
                heatable.ReceiveHeat(currentCharge);
                Debug.Log($"Applied {currentCharge} heat to {heatable.gameObject.name}");
                hitObject = true;
            }
        }

        if (!hitObject)
        {
            Debug.Log("Released heat but no heatable objects nearby");
        }

        // Reset charge state
        ResetCharge();
    }

    private void CancelCharge()
    {
        Debug.Log("Charge cancelled - not enough heat");
        ResetCharge();
    }

    private void ResetCharge()
    {
        currentCharge = 0f;
        isCharging = false;
        canRelease = false;
        playerController.SetUsingAbility(false);
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