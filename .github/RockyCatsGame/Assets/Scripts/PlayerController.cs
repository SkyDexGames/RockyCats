using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /*
    PlayerController should manage:
    1. Player movement
    2. Colliders
    3. Graphics
    
    Basically the client sided stuff
    */
    /*OTHER NOTES
    ------------------------------------
    si hay problemas de orientacion con los fbx del player, revisar linea
    targetRotation *= Quaternion.Euler(0, 180, 0);
    en HandleRotation

    puse un offset de 180 porque las orientaciones del modelo se estaban comportando raro y a pesar de intentar
    con diferentes orientaciones tanto en el unity como desde el blender, el modelo seguia mirando para atras o tenia offsets
    que la vdd no tengo ni idea de donde vienen
    ------------------------------------
    
    */

    //movement settings
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 1000; //rotate almost instantly, we can tweak this in the future if we need it

    //jump settings
    [SerializeField] private float jumpForce = 5f;


    //components
    private CharacterController controller;
    private Animator animator;
    
    //states
    private bool isGrounded = false;
    private float verticalVelocity;
    private Vector3 moveDirection;



    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovementInput();
        HandleJumpInput();
        ApplyGravity();
        ApplyMovement();
        HandleRotation();
        UpdateAnimations();
    }

    void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0f, vertical);
        moveDirection.Normalize();
    }

    void ApplyMovement()
    {
        Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
        move.y = verticalVelocity * Time.deltaTime;

        controller.Move(move);

        isGrounded = controller.isGrounded;
    }

    void HandleRotation()
    {
        if(moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            targetRotation *= Quaternion.Euler(0, 0, 0);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );

        }
    }

    void HandleJumpInput()
    {
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        verticalVelocity = jumpForce;
        isGrounded = false;
    }
    
    void ApplyGravity()
    {
        if(isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }

    void UpdateAnimations()
    {
        if(animator == null) return;

        int animationState = DetermineAnimationState();
        animator.SetInteger("State", animationState);
    }

    //cuando incluyamos los otros estados maybe le podemos meter un override o expandir esta funcion para darle la responsabilidad de las animaciones a una sola funcion, idk
    int DetermineAnimationState()
    {
        if(!isGrounded)
        {
            return 1; //jump
        }
        else if(moveDirection.magnitude > 0.01f)
        {
            return 2; //moving
        }
        else
        {
            return 0; //idle
        }
    }

    public bool IsGrounded => isGrounded;
    public Vector3 GetMoveDirection => moveDirection;
    public float GetMoveSpeed => moveSpeed;
}
