using UnityEngine;

public abstract class PlatformMovement : MonoBehaviour
{
    [Header("Parámetros comunes")]
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected bool loop = true;

    protected Vector3 startPos;

    protected virtual void Start()
    {
        startPos = transform.position;
    }

    protected abstract void Move();

    protected virtual void Update()
    {
        Move();
    }
}
