using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialShotWeapon : MonoBehaviour
{

    [SerializeField] private RadialShotPattern _shotPattern;

    [Header("Water SFX")]
    [SerializeField] private float waterSoundInterval = 0.2f;

    private bool _onShotPattern = false;
    private float waterSoundTimer = 0f;

    // private void Update()
    // {
    //     if (_onShotPattern)
    //         return;

    //     StartCoroutine(ExecuteRadialShotPattern(_shotPattern));
    // }

    public IEnumerator ExecuteRadialShotPattern(RadialShotPattern pattern)
    {
        if (_onShotPattern)
            yield break;
        _onShotPattern = true;
        int lap = 0;
        Vector3 aimDirection = transform.forward;
        Vector3 center = transform.position;

        yield return new WaitForSeconds(pattern.StartWait);

        while (lap < pattern.Repetitions)
        {
            if (lap > 0 && pattern.AngleOffsetBetweenReps != 0f)
                aimDirection = aimDirection.RotateAroundY(pattern.AngleOffsetBetweenReps);
            for (int i = 0; i < pattern.PatternSettings.Length; i++)
            {
                ShotAttack.RadialShot(center, aimDirection, pattern.PatternSettings[i]);
                PlayWaterSound();
                yield return new WaitForSeconds(pattern.PatternSettings[i].CoolDownAfterShot);
            }
            lap++;
        }

        yield return new WaitForSeconds(pattern.EndWait);
        _onShotPattern = false;
    }

    private void PlayWaterSound()
    {
        if (Time.time >= waterSoundTimer)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayLevelSFX("Water");
            waterSoundTimer = Time.time + waterSoundInterval;
        }
    }
}
