using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCONTOLD : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 100f;
    public float jumpSpeed = 5f;
    public float ySpeed = 5f;
    private CharacterController conn;
    private Animator animator;
    public bool isGrounded;

    void Start(){
        
    }

    void Update(){
        
        float horizontalMove = Input.GetAxis("Horizontal"); // A/D
        float verticalMove = Input.GetAxis("Vertical");     // W/S

        Vector3 moveDirection = new Vector3(horizontalMove, 0.0f, verticalMove);
        moveDirection.Normalize();
        float magnitude = moveDirection.magnitude;
        magnitude = Mathf.Clamp01(magnitude);
        conn.SimpleMove(moveDirection * magnitude * speed);

        
        ySpeed += Physics.gravity.y * Time.deltaTime;
        if(Input.GetButtonDown("Jump")){
            ySpeed = -0.5f;
            isGrounded = false;
        }
        
        Vector3 vel = moveDirection * magnitude;
        vel.y = ySpeed; 

        conn.Move(vel * Time.deltaTime);
        
        if(conn.isGrounded){
            ySpeed = -0.5f;
            isGrounded = true;
            if(Input.GetButtonDown("Jump")){
                ySpeed = jumpSpeed;
                isGrounded = false;
            }
        }
        
        
    
        if(moveDirection != Vector3.zero){
            Quaternion toRotate = Quaternion.LookRotation(moveDirection, Vector3.up);
            // Add 180 degree rotation to fix the backwards facing issue
            toRotate *= Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotate, rotationSpeed * Time.deltaTime);
        }

        UpdateAnimations(magnitude);
    }

    void UpdateAnimations(float movementMagnitude)
    {
        if (animator == null) return;
        
        int animationState;
        
        if (!isGrounded)
        {
            animationState = 1; // Jumping
        }
        else if (movementMagnitude > 0.1f)
        {
            animationState = 2; // Moving
        }
        else
        {
            animationState = 0; // Idle
        }
        
        animator.SetInteger("State", animationState);
    }
}