using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Rigidbody[] ragdollRigidbodies;
    public Animator animator;
    public CharacterController characterController;

    void Start()
    {
        SetRagdollActive(false);
    }

    public void Die()
    {
        SetRagdollActive(true);
        
        // Optional: add force for comedy
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
        }
    }

    void SetRagdollActive(bool active)
    {
        animator.enabled = !active;
        characterController.enabled = !active; // Disable Character Controller!
        
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !active;
        }
    }
}