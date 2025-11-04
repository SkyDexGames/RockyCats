using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RadialShotPatternVisualizer : MonoBehaviour
{
    [SerializeField] private RadialShotPattern _pattern;
    [SerializeField] private float _radius;
    [SerializeField] private Color _color;
    [SerializeField, Range(0f, 5f)] private float _testTime;

    private void OnDrawGizmos()
    {
        if (_pattern == null)
            return;
        Gizmos.color = _color;

        int lap = 0;
        Vector3 aimDirection = transform.forward;
        Vector3 center = transform.position;

        float timer = _testTime;


        while (timer > 0f && lap < _pattern.Repetitions)
        {
            if (lap > 0f && _pattern.AngleOffsetBetweenReps != 0f)
                aimDirection = aimDirection.RotateAroundY(_pattern.AngleOffsetBetweenReps);
                
            for (int i = 0; i < _pattern.PatternSettings.Length; i++)
            {

                if (timer < 0f)
                    break;
                DrawnRadialShot(_pattern.PatternSettings[i], timer, aimDirection);

                timer -= _pattern.PatternSettings[i].CoolDownAfterShot;
            }
            lap++;
        }
    }

    private void DrawnRadialShot(RadialShotSettings settings, float lifeTime, Vector3 aimDirection)
    {
        float angleBetweenBullets = 360f / settings.NumberOfBullets;
        if (settings.PhaseOffset != 0f || settings.AngleOffset != 0f)
            aimDirection = aimDirection.RotateAroundY((angleBetweenBullets * settings.PhaseOffset) + settings.AngleOffset);

        for (int i = 0; i < settings.NumberOfBullets; i++)
        {
            float bulletDirectionAngle = angleBetweenBullets * i;

            if (settings.RadialMask && bulletDirectionAngle > settings.MaskAngle)
                break;

            Vector3 bulletDirection = aimDirection.RotateAroundY(bulletDirectionAngle);
            Vector3 bulletPosition = transform.position + (bulletDirection * settings.BulletSpeed * lifeTime);
            Gizmos.DrawSphere(bulletPosition, _radius);
        }
    }
}
