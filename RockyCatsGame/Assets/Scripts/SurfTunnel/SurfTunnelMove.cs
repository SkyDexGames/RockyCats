using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfTunnelManager : MonoBehaviour
{
    
    void Update()
    {
        transform.position += new Vector3(0,0,-2) * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}
