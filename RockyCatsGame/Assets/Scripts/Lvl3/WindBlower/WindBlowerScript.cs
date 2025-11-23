using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBlowerScript : MonoBehaviour
{
    public float windDuration = 5f;
    
    public Vector3 localWindDirection = Vector3.forward;

    public GameObject windTriggerZone;
    public WindTriggerScript windTriggerScript;

    //private bool isBlowingWind = false;


    void Start()
    {
        if (windTriggerZone != null)
            windTriggerZone.SetActive(false);

        if (windTriggerScript == null && windTriggerZone != null)
            windTriggerScript = windTriggerZone.GetComponent<WindTriggerScript>();

        UpdateWindDirection();
    }

    private void OnEnable()
    {
        StartCoroutine(WindCycle());
    }

    public IEnumerator WindCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            
            StartBlowingWind();
            
            yield return new WaitForSeconds(windDuration);
            
            StopBlowingWind();
        }
    }

    void StartBlowingWind()
    {
        //isBlowingWind = true;
        
        if (windTriggerZone != null)
            windTriggerZone.SetActive(true);
        
        Debug.Log("Wind enemy started blowing wind");
    }

    void StopBlowingWind()
    {
        //isBlowingWind = false;
        
        if (windTriggerZone != null)
            windTriggerZone.SetActive(false);

        Debug.Log("Wind enemy stopped blowing wind");
        this.gameObject.SetActive(false);
    }

    public Vector3 GetWorldWindDirection()
    {
        return transform.TransformDirection(localWindDirection.normalized);
    }

    void UpdateWindDirection()
    {
        if (windTriggerScript != null)
        {
            windTriggerScript.windDirection = GetWorldWindDirection();
        }
    }

    public void UpdateWindFromRotation()
    {
        UpdateWindDirection();
    }
}