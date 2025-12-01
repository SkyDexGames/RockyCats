using UnityEngine;
using Photon.Pun;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType { Magma, Crystal }

    public ObstacleType obstacleType = ObstacleType.Magma;
    public int temperatureChange = 10;

    // Audio for looping TempUp sound while touching Magma
    private AudioSource tempUpAudioSource;
    private bool isLocalPlayerTouching = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();

            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                int finalTempChange = obstacleType == ObstacleType.Magma ? temperatureChange : -temperatureChange;

                if (Level1Manager.Instance != null)
                {
                    Level1Manager.Instance.UpdateMyTemperature(finalTempChange);
                }

                // Play SFX based on obstacle type
                if (AudioManager.Instance != null)
                {
                    if (obstacleType == ObstacleType.Magma)
                    {
                        // Subiendo temperatura - loop while touching
                        StartTempUpLoop();
                    }
                    else
                    {
                        // Crystal = daño (bajando temperatura)
                        AudioManager.Instance.PlayLevelSFX("Hurt");
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();

            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                if (obstacleType == ObstacleType.Magma)
                {
                    StopTempUpLoop();
                }
            }
        }
    }

    void StartTempUpLoop()
    {
        if (AudioManager.Instance == null) return;

        if (tempUpAudioSource == null)
        {
            tempUpAudioSource = gameObject.AddComponent<AudioSource>();
            tempUpAudioSource.loop = true;
            tempUpAudioSource.playOnAwake = false;
            tempUpAudioSource.spatialBlend = 0f;
        }

        var entry = AudioManager.Instance.GetLevelSFXEntry("TempUp");
        if (entry != null && entry.clip != null)
        {
            tempUpAudioSource.clip = entry.clip;
            tempUpAudioSource.volume = entry.volume * AudioManager.Instance.GetSFXVolume();
            tempUpAudioSource.Play();
            isLocalPlayerTouching = true;
        }
    }

    void StopTempUpLoop()
    {
        if (tempUpAudioSource != null && tempUpAudioSource.isPlaying)
        {
            tempUpAudioSource.Stop();
        }
        isLocalPlayerTouching = false;
    }

    void OnDisable()
    {
        if (isLocalPlayerTouching)
        {
            StopTempUpLoop();
        }
    }
}