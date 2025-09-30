using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
    ORIENTACION DEL MODELO

    si hay problemas de orientacion con los fbx del player, revisar linea
    targetRotation *= Quaternion.Euler(0, 180, 0);
    en HandleRotation

    puse un offset de 180 porque las orientaciones del modelo se estaban comportando raro y a pesar de intentar
    con diferentes orientaciones tanto en el unity como desde el blender, el modelo seguia mirando para atras o tenia offsets
    que la vdd no tengo ni idea de donde vienen

    ------------------------------------
    CHARACTER CONTROLLER OVER RIGIDBODY

    Estamos usando character controller en vez de rigidbody para el movimiento porque es 
    simplemente mejor en muchos sentidos, da mas modularidad, es mejor para multiplayer y se siente mas crisp, clean, y responsivo.
    
    El rigidbody es mas floaty y se siente muy raro para platformers.
    
    Nomas fue cuestion de agregar las funciones de gravedad, etc.
    ------------------------------------
    SEPARACION DE CONTROLLER Y FBX

    En teoria podriamos juntar el controller en un solo prefab, pero lo dejaremos separado, pues al menos este FBX no es la version final,
    y prefiero encapsular reponsabilidades (logica y animaciones)
    
    */

    //movement settings
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 1000; //rotate almost instantly, we can tweak this in the future if we need it

    //jump settings
    [SerializeField] private float jumpForce = 5f;

    //ground check
    [SerializeField] private bool useBoxGroundCheck = true;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundMask = ~0;

    //components
    private CharacterController controller;
    [SerializeField] Animator animator;
    
    //states
    private bool isGrounded;
    private float verticalVelocity;
    private Vector3 moveDirection;

    PhotonView PV;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if(!PV.IsMine) return; //para solo controlar a NUESTRA instance del player
        HandleMovementInput();
        HandleJumpInput();
        ApplyGravity();
        ApplyMovement();
        HandleRotation();
        UpdateAnimations();
        Debug.Log("IsGrounded: " + isGrounded);
    }

    void HandleMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Store the raw input separately
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);
        
        // Only update moveDirection if there's actual input
        if(inputDirection.magnitude > 0.01f)
        {
            moveDirection = inputDirection.normalized;
        }
        else
        {
            moveDirection = Vector3.zero; // Immediately zero out
        }
    }

    void ApplyMovement()
    {
        Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
        move.y = verticalVelocity * Time.deltaTime;

        controller.Move(move);

        if(useBoxGroundCheck)
        {
            CheckGroundedBox();
        }
        else
        {
            isGrounded = controller.isGrounded;
        }
    }

    void CheckGroundedBox()
    {
        // Box check covers the entire bottom of the character
        Vector3 boxCenter = transform.position + Vector3.up * (groundCheckDistance / 2);
        Vector3 boxHalfExtents = new Vector3(controller.radius * 0.9f, groundCheckDistance / 2, controller.radius * 0.9f);
        
        isGrounded = Physics.CheckBox(boxCenter, boxHalfExtents, Quaternion.identity, groundMask);
    }

    void HandleRotation()
    {
        // Only rotate if there's input THIS frame
        if(moveDirection.magnitude > 0.01f)
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
        if (isGrounded && verticalVelocity <= 0)
        {
            verticalVelocity = -0.5f; // Small downward force to keep grounded
        }
        
        if (!isGrounded || verticalVelocity > 0)
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


    void OnDrawGizmos()
    {
        if (!Application.isPlaying || controller == null || !useBoxGroundCheck) return;
        
        Vector3 boxCenter = transform.position + Vector3.up * (groundCheckDistance / 2);
        Vector3 boxSize = new Vector3(controller.radius * 1.8f, groundCheckDistance, controller.radius * 1.8f);
        
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
