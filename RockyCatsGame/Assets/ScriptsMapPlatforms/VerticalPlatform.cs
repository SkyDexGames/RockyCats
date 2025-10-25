using UnityEngine;

public class VerticalPlatform : PlatformMovement
{
    [Header("Movimiento vertical")]
    [SerializeField] private float amplitude = 3f; // Distancia total del recorrido
    [SerializeField] private bool moveUpFirst = true; // Si inicia moviéndose hacia arriba

    private float direction = 1f; // 1 = sube, -1 = baja
    private float topLimit;
    private float bottomLimit;

    protected override void Start()
    {
        base.Start();

        // La posición inicial donde la coloques es el punto base
        float baseY = startPos.y;

        // Calcula los límites superior e inferior
        if (moveUpFirst)
        {
            topLimit = baseY + amplitude;
            bottomLimit = baseY;
            direction = 1f;
        }
        else
        {
            topLimit = baseY;
            bottomLimit = baseY - amplitude;
            direction = -1f;
        }
    }

    protected override void Move()
    {
        transform.Translate(Vector3.up * direction * speed * Time.deltaTime);

        // Si llegó a los límites, invierte la dirección
        if (direction > 0 && transform.position.y >= topLimit)
        {
            transform.position = new Vector3(transform.position.x, topLimit, transform.position.z);
            direction = -1f;
        }
        else if (direction < 0 && transform.position.y <= bottomLimit)
        {
            transform.position = new Vector3(transform.position.x, bottomLimit, transform.position.z);
            direction = 1f;
        }
    }
}
