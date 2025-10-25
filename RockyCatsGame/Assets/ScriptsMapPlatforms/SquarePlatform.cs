using UnityEngine;

public class SquarePlatform : PlatformMovement
{
    public enum StartDirection
    {
        Right_Forward,   // → ↑
        Right_Backward,  // → ↓
        Left_Forward,    // ← ↑
        Left_Backward    // ← ↓
    }

    [Header("Movimiento cuadrado / rectangular")]
    [SerializeField] private float width = 5f;   // Distancia en eje X
    [SerializeField] private float depth = 5f;   // Distancia en eje Z
    [SerializeField] private float pauseTime = 0.2f; // pausa opcional en esquinas
    [SerializeField] private StartDirection startDirection = StartDirection.Right_Forward;

    private Vector3[] corners;
    private int targetIndex = 0;
    private float pauseTimer = 0f;

    protected override void Start()
    {
        base.Start();

        Vector3 p = startPos;

        // Define las esquinas según el ancho y alto
        corners = new Vector3[4];

        switch (startDirection)
        {
            case StartDirection.Right_Forward:
                corners[0] = p + new Vector3(width, 0, 0);          // derecha
                corners[1] = p + new Vector3(width, 0, depth);      // derecha-frente
                corners[2] = p + new Vector3(0, 0, depth);          // frente
                corners[3] = p;                                     // regresa
                break;

            case StartDirection.Right_Backward:
                corners[0] = p + new Vector3(width, 0, 0);          // derecha
                corners[1] = p + new Vector3(width, 0, -depth);     // derecha-atrás
                corners[2] = p + new Vector3(0, 0, -depth);         // atrás
                corners[3] = p;                                     // regresa
                break;

            case StartDirection.Left_Forward:
                corners[0] = p + new Vector3(-width, 0, 0);         // izquierda
                corners[1] = p + new Vector3(-width, 0, depth);     // izquierda-frente
                corners[2] = p + new Vector3(0, 0, depth);          // frente
                corners[3] = p;                                     // regresa
                break;

            case StartDirection.Left_Backward:
                corners[0] = p + new Vector3(-width, 0, 0);         // izquierda
                corners[1] = p + new Vector3(-width, 0, -depth);    // izquierda-atrás
                corners[2] = p + new Vector3(0, 0, -depth);         // atrás
                corners[3] = p;                                     // regresa
                break;
        }
    }

    protected override void Move()
    {
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        Vector3 target = corners[targetIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Si llega a una esquina, pasa a la siguiente
        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            targetIndex = (targetIndex + 1) % corners.Length;

            if (pauseTime > 0f)
                pauseTimer = pauseTime;
        }
    }
}
