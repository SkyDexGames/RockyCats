using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.TextCore.Text;
using ExitGames.Client.Photon.StructWrapping;

public class IgneousPhase : PlayerPhase
{

    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCoolDown = 2f;
    private PhotonView photonView;

    private bool isDashing = false;
    private bool canDash = true;
    private float dashTimer = 0f;

    private CharacterController characterController;

    public override void Initialize(PlayerController controller)
    {
        base.Initialize(controller);
        characterController = controller.GetController();
        photonView = controller.GetComponent<PhotonView>();
    }
    void OnEnable()
    {
        if (!canDash && !isDashing)
        {
            canDash = true;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isDashing&& photonView.IsMine)
        {
            CheckIgneousInteractable(hit.gameObject);
        }
    }

    private void CheckIgneousInteractable(GameObject obj)
    {
        //buscar si el objeto con el que estamos colisionando si tiene interfaz
        IIgneousInteractable interactable = obj.GetComponent<IIgneousInteractable>();
        
        if (interactable != null)
        {
            interactable.OnIgneousCollision();
        }
    }

    public override void HandleAbility()
    {
        if (canDash && !isDashing && photonView.IsMine)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        dashTimer = 0f;

        if (playerController != null)
        {
            playerController.SetUsingAbility(true);
        }


        Vector3 dashDirection = playerController.transform.forward;

        Debug.Log("Igneous dash activatedddddd");

        while (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;

            Vector3 dashMovement = dashDirection * dashForce * Time.deltaTime;

            if (photonView.IsMine)
            {
                characterController.Move(dashMovement);
            }

            yield return null;
        }

        isDashing = false;

        if (playerController != null)
        {
            playerController.SetUsingAbility(false);
        }

        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;

        Debug.Log("Dash ready again");
    }

    public override void HandleMovement(ref Vector3 moveDirection, ref float moveSpeed)
    {
        if (isDashing && photonView.IsMine)
        {
            moveDirection = Vector3.zero;
        }
    }

    public override bool IsUsingAbility()
    {
        return isDashing;
    }

    //getters
    public bool IsDashing => isDashing;
    public bool CanDash => canDash;
    public float DashCooldownProgress => canDash ? 1f : (dashTimer / dashCoolDown);

}
