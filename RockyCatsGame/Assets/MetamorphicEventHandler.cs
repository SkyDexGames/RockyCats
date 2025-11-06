using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetamorphicEventHandler : MonoBehaviour
{   
    [SerializeField] private GameObject surfBoard;
    
    public void SetSurfBoardActive()
    {
        surfBoard.SetActive(true);  
    }
}
