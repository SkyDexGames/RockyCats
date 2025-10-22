using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finalHoleScript : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.position += new Vector3(0, 0, -20) * Time.deltaTime;
    }
}
