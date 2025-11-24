using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 1000; //rotate almost instantly, we can tweak this in the future if we need it
    [SerializeField] private float groundAcceleration = 50f; //que tan rapido aceleras
    [SerializeField] private float groundDeceleration = 30f; //que tan rapido desaceleras
    [SerializeField] private float airAcceleration = 20f; //control en el aire

    //jump settings
    [SerializeField] private float jumpForce = 5f;

    //some input settings
    [SerializeField] private bool useController = true;
    [SerializeField] private float controllerDeadzone = 0.2f;

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
    [SerializeField] private Animator[] phaseAnimators;
    private Animator currentAnimator;

    //weas de movimiento, separamos el input de la velocidad para aplicar los efectos de manera no tosca
    private Vector3 inputDirection;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    //states for physics and anims
    private bool isGrounded;
    private bool isTouchingWall;

    //gravity settings
    [SerializeField] private float normalGravity = -10f;
    [SerializeField] private float surfingGravity = -10f;
    [SerializeField] private float haltedGravity = -10f;
    private float currentGravity;

    //weas externas que afectan el movimiento
    private Vector3 externalVelocity;
    private float externalVelocityTimer;

    //photon y otros sistemas
    PhotonView PV;
    private PhaseManager phaseManager; //eta vaina va a manejar las fases Y las va a comunicar con photon
    private bool isUsingAbility = false;

    //respawn pos
    private Vector3 currentSpawnpoint;

    private int hp;

    private int maxHp = 100;

    public bool isDead = false;


    //states for diff game modes (this will be refactored later into abstracts probs)

    public enum MovementMode { Normal, Surfing, Halted }

    [SerializeField] private MovementMode currentMovementMode = MovementMode.Normal;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
        phaseManager = GetComponent<PhaseManager>();
        currentGravity = normalGravity;
        hp = maxHp;
    }

    void Update()
    {
        if (!PV.IsMine) return;

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
        
        if (useController)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            if (Mathf.Abs(horizontal) < controllerDeadzone) horizontal = 0f;
            if (Mathf.Abs(vertical) < controllerDeadzone) vertical = 0f;
        }

        if (Mathf.Abs(horizontal) < 0.01f && Mathf.Abs(vertical) < 0.01f)
        {
            if (Input.GetKey(moveLeft)) horizontal -= 1f;
            if (Input.GetKey(moveRight)) horizontal += 1f;

            if (currentMovementMode != MovementMode.Surfing)
            {
                if (Input.GetKey(moveBackward)) vertical -= 1f;
                if (Input.GetKey(moveForward)) vertical += 1f;
            }
        }
        else
        {
            if (currentMovementMode == MovementMode.Surfing)
            {
                vertical = 0f;
            }
        }
        Vector3 rawInput = new Vector3(horizontal, 0f, vertical);
        inputDirection = TransformInputRelativeToCamera(rawInput);
    }

    void UpdateVelocity()
    {
        //cuanto control tiene el jugador
        float controlMultiplier = 1f;
        
        //reducimos control durante el wall jump para que SI se apliquen bien las fuerzas
        //solo durante el wall jump, porque con el viento se sentia bien unresponsive
        if(externalVelocityTimer > 0 && externalVelocity.magnitude > wallJumpSideForce * 0.5f)
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

    public void SetCurrentAnimator(Animator newAnimator)
    {
        currentAnimator = newAnimator;
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

        bool jumpPressed = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space) ||Input.GetKeyDown(KeyCode.JoystickButton0);


        if(jumpPressed && isGrounded)
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
            verticalVelocity += currentGravity * Time.deltaTime;
        }
    }

    void UpdateAnimations()
    {
        if(currentAnimator == null) return;

        bool isDashing = false;
        if (phaseManager != null)
        {
            var currentPhase = phaseManager.GetCurrentPhase();
            if (currentPhase != null)
            {
                isDashing = currentPhase.IsUsingAbility();
                // Or check specifically for IgneousPhase
                if (currentPhase is IgneousPhase igneousPhase)
                {
                    isDashing = igneousPhase.IsDashing;
                }
            }
        }

        bool isSurfing = currentMovementMode == MovementMode.Surfing;

        if (!isDead)
        {
            currentAnimator.SetBool("IsGrounded", isGrounded);
            currentAnimator.SetFloat("MoveSpeed", horizontalVelocity.magnitude);
            currentAnimator.SetFloat("VerticalVelocity", verticalVelocity);
            currentAnimator.SetBool("IsDashing", isDashing);
            currentAnimator.SetBool("isSurfing", isSurfing);
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
    
    public void SetExternalVelocity(Vector3 velocity, float duration)
    {
        externalVelocity = velocity;
        externalVelocityTimer = duration;
    }

    public void SetMovementMode(MovementMode mode) 
    { 
        currentMovementMode = mode;
        
        switch (mode)
        {
            case MovementMode.Normal:
                currentGravity = normalGravity;
                break;
            case MovementMode.Surfing:
                currentGravity = surfingGravity;
                break;
            case MovementMode.Halted:
                currentGravity = haltedGravity;
                break;
        }
        
        if (mode == MovementMode.Halted)
        {
            horizontalVelocity = Vector3.zero;
            inputDirection = Vector3.zero;
        }
        
        if (mode == MovementMode.Surfing) 
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
            
    }
    public void SetAnimator(int phaseIndex)
    {
        if (phaseIndex >= 0 && phaseIndex < phaseAnimators.Length)
        {
            currentAnimator = phaseAnimators[phaseIndex];
        }
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

    public Animator GetCurrentAnimator()
    {
        return currentAnimator;
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

    public void TakeDamage(int damage)
    {
        if (!PV.IsMine || isDead) return;
        
        hp = Mathf.Max(0, hp - damage);
        LevelManager.Instance.UpdateMyTemperature(hp);
        if (hp <= 0)
        {
            
            Die();
        }
    }
    void Die()
    {
        if (!PV.IsMine || isDead) return;

        isDead = true;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash["isDead"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);


        currentMovementMode = MovementMode.Halted;
        currentAnimator.SetTrigger("Die");
        
        StartCoroutine(RespawnRoutine());
            
    }


    private IEnumerator RespawnRoutine()
    {
        if (!PV.IsMine) yield break;
        yield return new WaitForSeconds(2f);

        controller.enabled = false;
        transform.position = LevelManager.Instance.GetDeathPoint() + new Vector3(0, 3, 0);
        controller.enabled = true;

        yield return new WaitForSeconds(5f);

        Vector3 spawnPos = LevelManager.Instance.GetSpawnPoint() ;
        OnRespawn(spawnPos);
    }

   
    void OnRespawn(Vector3 pos)
    {
        if (!PV.IsMine) return;
        hp = 100;
        LevelManager.Instance.UpdateMyTemperature(hp);
        controller.enabled = false;
        transform.position = pos;
        controller.enabled = true;
        isDead = false;
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash["isDead"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        currentMovementMode = MovementMode.Normal;

    }
}