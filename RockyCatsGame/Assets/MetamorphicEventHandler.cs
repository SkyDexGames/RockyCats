using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetamorphicEventHandler : MonoBehaviour
{   
    [SerializeField] private GameObject surfBoard;
    [SerializeField] private Animator animator;
    
    public void SetSurfBoardActive()
    {
        surfBoard.SetActive(true);
    }
    
    public void SetOllieTrue()
    {
        animator.SetBool("Ollie", true);
    }
    
    public void SetOllieFalse()
    {
        animator.SetBool("Ollie", false);
    }
}