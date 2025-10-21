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
    

    --------------
    REFACTOR DEL MOVIMIENTO

    ANTES el input era indicacion directa de movimiento y eso daba problemas al aplicar fuerzas externas, como lo son el caso del wall jump

    AHORA el input solo indica direccion y la velocidad se maneja aparte, esto nos da chance de aplicar efectos al movimiento sin que el input este sobreescribiendo todo cada frame.
    */
    //movement settings
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 1000; //rotate almost instantly, we can tweak this in the future if we need it
    [SerializeField] private float groundAcceleration = 50f; //que tan rapido aceleras
    [SerializeField] private float groundDeceleration = 30f; //que tan rapido desaceleras
    [SerializeField] private float airAcceleration = 20f; //control en el aire

    //jump settings
    [SerializeField] private float jumpForce = 5f;

    //input keys
    [SerializeField] private KeyCode moveLeft = KeyCode.A;
    [SerializeField] private KeyCode moveRight = KeyCode.D;
    [SerializeField] private KeyCode moveForward = KeyCode.W;
    [SerializeField] private KeyCode moveBackward = KeyCode.S;

    //ground check
    [SerializeField] private bool useBoxGroundCheck = true;
    [SerializeField] private float groundCheckDistance = 0.2f;

    //layers para los triggers
    [SerializeField] private LayerMask groundMask; //por si queremos manejar con layers
    [SerializeField] private LayerMask killZoneMask;
    [SerializeField] private LayerMask cameraTriggerMask;

    //wall jump
    [SerializeField] private float wallJumpUpForce = 8f;
    [SerializeField] private float wallJumpSideForce = 10f;
    [SerializeField] private float wallJumpDuration = 0.3f;
    [SerializeField] private float wallJumpControlReduction = 0.2f;

    //components
    private CharacterController controller;
    [SerializeField] Animator animator;

    //weas de movimiento, separamos el input de la velocidad para aplicar los efectos de manera no tosca
    private Vector3 inputDirection;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    //states for physics and anims
    private bool isGrounded;
    private bool isTouchingWall;

    //weas externas que afectan el movimiento
    private Vector3 externalVelocity;
    private float externalVelocityTimer;

    //photon y otros sistemas
    PhotonView PV;
    private PhaseManager phaseManager; //eta vaina va a manejar las fases Y las va a comunicar con photon
    private bool isUsingAbility = false;

    //respawn pos
    private Vector3 currentSpawnpoint;


    //states for diff game modes (this will be refactored later into abstracts probs)

    public enum MovementMode { Normal, Surfing, Halted }

    [SerializeField] private MovementMode currentMovementMode = MovementMode.Normal;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
        phaseManager = GetComponent<PhaseManager>();
    }

    void Update()
    {
        if(!PV.IsMine) return;

        CaptureInput();
        HandleJumpInput();
        UpdateVelocity();
        ApplyGravity();
        ApplyMovement();

        HandleRotation();

        //checamos si colisiona con la killzone en vez de llamar a ooootro metodo como andabamos haciendo
        if (CheckCollisionWithLayer(killZoneMask, controller.height / 2, controller.height / 2))
        {
            RespawnAtCheckpoint();
        }

        UpdateAnimations();

        //vamos matando poco a poco la velocidad externa
        if (externalVelocityTimer > 0)
        {
            externalVelocityTimer -= Time.deltaTime;
            if (externalVelocityTimer <= 0)
            {
                externalVelocity = Vector3.zero;
            }
        }
    }

    void CaptureInput()
    {
        
        if (isUsingAbility || currentMovementMode == MovementMode.Halted)
        {
            inputDirection = Vector3.zero;
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;
        
        if (Input.GetKey(moveLeft)) horizontal -= 1f;
        if (Input.GetKey(moveRight)) horizontal += 1f;

        if(currentMovementMode != MovementMode.Surfing)
        {
            if (Input.GetKey(moveBackward)) vertical -= 1f;
            if (Input.GetKey(moveForward)) vertical += 1f;
        }
        Vector3 rawInput = new Vector3(horizontal, 0f, vertical);
        inputDirection = TransformInputRelativeToCamera(rawInput);
    }

    void UpdateVelocity()
    {
        //cuanto control tiene el jugador
        float controlMultiplier = 1f;
        
        //reducimos control durante el wall jump para que SI se apliquen bien las fuerzas
        if(externalVelocityTimer > 0)
        {
            controlMultiplier = wallJumpControlReduction;
        }

        //target velocity basado en input
        Vector3 targetVelocity = inputDirection * moveSpeed * controlMultiplier;
        
        //acelerar hacia targetVelocity
        float acceleration = isGrounded ? groundAcceleration : airAcceleration;
        
        if(inputDirection.magnitude > 0.01f)
        {
            //acelerando hacia targetVelocity
            horizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity, 
                targetVelocity, 
                acceleration * Time.deltaTime
            );
        }
        else
        {
            //decelerating a 0
            float deceleration = isGrounded ? groundDeceleration : airAcceleration * 0.5f;
            horizontalVelocity = Vector3.MoveTowards(
                horizontalVelocity, 
                Vector3.zero, 
                deceleration * Time.deltaTime
            );
        }
    }

    void ApplyMovement()
    {
        //mezclar todas las velocidades
        Vector3 totalHorizontalVelocity = horizontalVelocity + externalVelocity;
        
        Vector3 movement = totalHorizontalVelocity * Time.deltaTime;
        movement.y = verticalVelocity * Time.deltaTime;

        controller.Move(movement);

        if(useBoxGroundCheck)
        {
            isGrounded = CheckCollisionWithLayer(groundMask, groundCheckDistance / 2, groundCheckDistance / 2);
        }
        else
        {
            isGrounded = controller.isGrounded;
        }
    }
    //checker de colisiones generico ahora si hehe
    private bool CheckCollisionWithLayer(LayerMask layerMask, float verticalOffset, float verticalHalfExtent)
    {
        Vector3 boxCenter = transform.position + Vector3.up * verticalOffset;
        Vector3 boxHalfExtents = new Vector3(
            controller.radius * 0.9f, 
            verticalHalfExtent, 
            controller.radius * 0.9f
        );
        
        return Physics.CheckBox(boxCenter, boxHalfExtents, Quaternion.identity, layerMask);
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (currentMovementMode != MovementMode.Normal) return;

        if(!isGrounded && hit.normal.y < 0.1f)
        {
            isTouchingWall = true;
            
            if(Input.GetButtonDown("Jump"))
            {
                PerformWallJump(hit.normal);
            }
        }

        //phase manager wall slide notification
        if (phaseManager != null)
        {
            var currentPhase = phaseManager.GetCurrentPhase();
            if (currentPhase != null)
            {
                currentPhase.HandleWallSlide(isTouchingWall);
            }
        }
    }

    void PerformWallJump(Vector3 wallNormal)
    {
        verticalVelocity = wallJumpUpForce;
        
        //aqui le hacemos para que rebote en la dir contraria a la pared
        Vector3 jumpDirection = new Vector3(wallNormal.x, 0, wallNormal.z).normalized;
        externalVelocity = jumpDirection * wallJumpSideForce;
        externalVelocityTimer = wallJumpDuration;
    
        //horizontalVelocity = Vector3.zero;
        
        //Debug.Log($"Wall Jump direction: {jumpDirection}, force: {wallJumpSideForce}");
    }

    void HandleRotation()
    {
        //Rotar hacia donde nos estamos moviendo
        Vector3 movementDirection = horizontalVelocity + externalVelocity;

        if (currentMovementMode == MovementMode.Surfing) 
        movementDirection = Vector3.forward;
        if (currentMovementMode == MovementMode.Halted) 
        return;
        
        if(movementDirection.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void HandleJumpInput()
    {

        if (currentMovementMode == MovementMode.Halted) return;

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
            verticalVelocity = -0.5f;
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

    int DetermineAnimationState()
    {
        if(!isGrounded)
        {
            return 1; // jump
        }
        else if(horizontalVelocity.magnitude > 0.5f)
        {
            return 2; // moving
        }
        else
        {
            return 0; // idle
        }
    }

    //spawns
    void SetInitialSpawnPoint()
    {
        currentSpawnpoint = transform.position;
    }

    void RespawnAtCheckpoint()
    {
        horizontalVelocity = Vector3.zero;
        verticalVelocity = 0f;
        externalVelocity = Vector3.zero;
        externalVelocityTimer = 0f;
        inputDirection = Vector3.zero;
        isGrounded = false;
        isTouchingWall = false;
        transform.position = currentSpawnpoint;
    }

    public void SetSpawnPoint(Vector3 newSpawnPoint)
    {
        if (PV.IsMine)
        {
            currentSpawnpoint = newSpawnPoint;
        }
    }

    //getters
    public bool IsGrounded => isGrounded;
    public bool IsTouchingWall => isTouchingWall;
    public Vector3 GetInputDirection => inputDirection;
    public Vector3 GetHorizontalVelocity => horizontalVelocity;
    public float GetMoveSpeed => moveSpeed;
    public float GetVerticalVelocity() => verticalVelocity;
    public CharacterController GetController() => controller;

    //setters
    public void SetMoveSpeed(float speed) => moveSpeed = speed;
    public void SetJumpForce(float force) => jumpForce = force;
    public void SetRotationSpeed(float speed) => rotationSpeed = speed;
    public void SetUsingAbility(bool usingAbility) => isUsingAbility = usingAbility;
    public void SetVerticalVelocity(float velocity) => verticalVelocity = velocity;
    
    public void AddExternalVelocity(Vector3 velocity, float duration)
    {
        externalVelocity = velocity;
        externalVelocityTimer = duration;
    }

    public void SetMovementMode(MovementMode mode) 
    { 
        currentMovementMode = mode;
        
        if (mode == MovementMode.Halted)
        {
            horizontalVelocity = Vector3.zero;
            inputDirection = Vector3.zero;
        }
        
        if (mode == MovementMode.Surfing) 
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
    }

    public void SetToNormal() => SetMovementMode(MovementMode.Normal);
    public void SetToSurfing() => SetMovementMode(MovementMode.Surfing);  
    public void SetToHalted() => SetMovementMode(MovementMode.Halted);

    Vector3 TransformInputRelativeToCamera(Vector3 input)
    {
        if (input.magnitude < 0.01f) return Vector3.zero;
        
        Camera cam = Camera.main;
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        
        camForward.y = 0f;
        camRight.y = 0f;
        
        camForward.Normalize();
        camRight.Normalize();
        
        Vector3 worldInput = camRight * input.x + camForward * input.z;
        
        return worldInput.normalized;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying || controller == null) return;
        
        if (useBoxGroundCheck)
        {
            Vector3 boxCenter = transform.position + Vector3.up * (groundCheckDistance / 2);
            Vector3 boxSize = new Vector3(
                controller.radius * 1.8f, 
                groundCheckDistance, 
                controller.radius * 1.8f
            );
            
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
    }
}