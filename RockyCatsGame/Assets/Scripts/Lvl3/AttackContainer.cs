using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackContainer : MonoBehaviour
{
    public RadialShotWeapon weapon;


    public IEnumerator ExecutePattern(RadialShotPattern pattern)
    {
        yield return weapon.StartCoroutine(weapon.ExecuteRadialShotPattern(pattern));
    }


    public void ShootWind()
    {
        return;
    }
}