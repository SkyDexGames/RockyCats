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

    private AudioSource windAudioSource;


    void Start()
    {
        if (windTriggerZone != null)
            windTriggerZone.SetActive(false);

        if (windTriggerScript == null && windTriggerZone != null)
            windTriggerScript = windTriggerZone.GetComponent<WindTriggerScript>();

        UpdateWindDirection();
        SetupWindAudio();
    }

    void SetupWindAudio()
    {
        windAudioSource = gameObject.AddComponent<AudioSource>();
        windAudioSource.loop = true;
        windAudioSource.playOnAwake = false;

        // Obtener el clip y volumen del LevelAudioConfig
        if (AudioManager.Instance != null)
        {
            var entry = AudioManager.Instance.GetLevelSFXEntry("WindBlowing");
            if (entry != null)
            {
                windAudioSource.clip = entry.clip;
                windAudioSource.volume = entry.volume * AudioManager.Instance.GetSFXVolume();
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(WindCycle());
    }

    private void OnDisable()
    {
        if (windAudioSource != null && windAudioSource.isPlaying)
            windAudioSource.Stop();
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

        // Iniciar sonido de viento en loop
        if (windAudioSource != null && windAudioSource.clip != null)
            windAudioSource.Play();

        Debug.Log("Wind enemy started blowing wind");
    }

    void StopBlowingWind()
    {
        //isBlowingWind = false;

        if (windTriggerZone != null)
            windTriggerZone.SetActive(false);

        // Detener sonido de viento
        if (windAudioSource != null && windAudioSource.isPlaying)
            windAudioSource.Stop();

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