using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTest : MonoBehaviour
{

    [SerializeField] private float _shootCooldown;
    [SerializeField] private RadialShotSettings _shotSettings;

    private float _shootCooldownTimer = 0f;

    private void Update()
    {
        _shootCooldownTimer -= Time.deltaTime;

        if(_shootCooldownTimer <= 0f)
        {
            ShotAttack.RadialShot(transform.position, transform.up,  _shotSettings);
            _shootCooldownTimer += _shootCooldown;
        }
    }
}
