using System.Collections;
using UnityEngine;
using Photon.Pun;

public class GasCraterController : MonoBehaviourPun
{
    [Range(0, 3)] public int craterId = 0;

    [Header("Referencias Visuales (opcionales)")]
    [SerializeField] private ParticleSystem gasVFX;
    [SerializeField] private AudioSource gasSFX;

    public void PlayGas(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(CoPlayGas(duration));
    }

    private IEnumerator CoPlayGas(float duration)
    {
        if (gasVFX != null) gasVFX.Play(true);
        if (gasSFX != null) gasSFX.Play();

        yield return new WaitForSeconds(duration);

        if (gasVFX != null) gasVFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (gasSFX != null) gasSFX.Stop();
    }
}

