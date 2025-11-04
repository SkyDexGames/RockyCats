using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private const float MAX_LIFE_TIME = 3f;
    private float _lifeTime = 0f;
    public Vector3 Velocity;

    private void Update()
    {
        Vector3 newPosition = transform.position + Velocity * Time.deltaTime;
        transform.position = newPosition;
        _lifeTime += Time.deltaTime;

        if(_lifeTime > MAX_LIFE_TIME)
            Disable();
    }

    private void Disable()
    {
        _lifeTime = 0f;
        gameObject.SetActive(false);
    }
}
