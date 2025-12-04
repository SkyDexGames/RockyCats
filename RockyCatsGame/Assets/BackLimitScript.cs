using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackLimitScript : MonoBehaviour
{
    public BoxCollider boxCollider;
    
    void Start()
    {
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }
    }
}
