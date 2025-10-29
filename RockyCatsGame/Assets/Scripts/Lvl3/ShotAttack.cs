using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShotAttack
{
    public static void SimpleShot (Vector3 origin, Vector3 velocity)
    {
        Bullet bullet = BulletPool.Instance.RequestBullet();
        bullet.transform.position = origin;
        bullet.Velocity = velocity;
    }

    public static void RadialShot (Vector3 origin, Vector3 aimDirection, RadialShotSettings settings)
    {
        float angleBetweenBullets = 360f / settings.NumberOfBullets;

        if (settings.AngleOffset != 0f)
            aimDirection = aimDirection.RotateAroundY(settings.AngleOffset + (settings.PhaseOffset * angleBetweenBullets));

        for(int i = 0; i < settings.NumberOfBullets; i++)
        {
            float bulletDirectionAngle = angleBetweenBullets * i;

            if (settings.RadialMask && bulletDirectionAngle > settings.MaskAngle)
                break;
            Vector3 bulletDirection = aimDirection.RotateAroundY(bulletDirectionAngle);
            SimpleShot(origin, bulletDirection * settings.BulletSpeed);
        }
    }

}
