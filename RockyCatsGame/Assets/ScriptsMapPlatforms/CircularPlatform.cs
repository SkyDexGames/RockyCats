using UnityEngine;

public class CircularPlatform : PlatformMovement
{
    public enum MotionMode { FullCircle, Swing }
    public enum RotationPlane { XZ, XY, YZ }
    public enum RotationDirection { CounterClockwise, Clockwise }

    [Header("General")]
    [SerializeField] private MotionMode mode = MotionMode.FullCircle;
    [SerializeField] private RotationPlane rotationPlane = RotationPlane.XZ;
    [SerializeField] private RotationDirection rotationDirection = RotationDirection.CounterClockwise;
    [SerializeField] private float radius = 3f;

    [Header("Swing (modo péndulo)")]
    [SerializeField, Range(0f, 90f)] private float swingAngle = 45f;

    [Header("Posición inicial")]
    [SerializeField, Range(0f, 360f)] private float startAngle = 0f; // 0° = abajo

    private Vector3 pivotPoint;
    private float dirSign;

    protected override void Start()
    {
        base.Start();

        // Sentido del movimiento (horario o antihorario)
        dirSign = (rotationDirection == RotationDirection.CounterClockwise) ? 1f : -1f;

        // El pivote es el centro del círculo o punto de suspensión
        pivotPoint = startPos + Vector3.up * radius;

        // Convertimos el ángulo inicial de grados a radianes
        startAngle = Mathf.Deg2Rad * startAngle;
    }

    protected override void Move()
    {
        if (mode == MotionMode.FullCircle)
            MoveFullCircle();
        else
            MoveSwing();
    }

    // Círculo completo
    private void MoveFullCircle()
    {
        float angle = startAngle + Time.time * speed * dirSign;

        switch (rotationPlane)
        {
            case RotationPlane.XZ:
                transform.position = new Vector3(
                    pivotPoint.x + Mathf.Cos(angle) * radius,
                    pivotPoint.y,
                    pivotPoint.z + Mathf.Sin(angle) * radius
                );
                break;

            case RotationPlane.XY:
                transform.position = new Vector3(
                    pivotPoint.x + Mathf.Sin(angle) * radius,
                    pivotPoint.y - Mathf.Cos(angle) * radius,
                    pivotPoint.z
                );
                break;

            case RotationPlane.YZ:
                transform.position = new Vector3(
                    pivotPoint.x,
                    pivotPoint.y - Mathf.Cos(angle) * radius,
                    pivotPoint.z + Mathf.Sin(angle) * radius
                );
                break;
        }
    }

    // Movimiento tipo péndulo
    private void MoveSwing()
    {
        float swing = Mathf.Sin(Time.time * speed * dirSign) * swingAngle;
        float swingRad = swing * Mathf.Deg2Rad;

        switch (rotationPlane)
        {
            case RotationPlane.XZ:
                transform.position = new Vector3(
                    pivotPoint.x + Mathf.Sin(swingRad) * radius,
                    pivotPoint.y - Mathf.Cos(swingRad) * radius,
                    pivotPoint.z
                );
                break;

            case RotationPlane.XY:
                transform.position = new Vector3(
                    pivotPoint.x + Mathf.Sin(swingRad) * radius,
                    pivotPoint.y - Mathf.Cos(swingRad) * radius,
                    pivotPoint.z
                );
                break;

            case RotationPlane.YZ:
                transform.position = new Vector3(
                    pivotPoint.x,
                    pivotPoint.y - Mathf.Cos(swingRad) * radius,
                    pivotPoint.z + Mathf.Sin(swingRad) * radius
                );
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = (mode == MotionMode.FullCircle) ? Color.cyan : Color.yellow;
        Vector3 pivot = Application.isPlaying ? pivotPoint : transform.position + Vector3.up * radius;

        // Dibuja siempre el pivote y la cuerda
        Gizmos.DrawWireSphere(pivot, 0.1f);
        Gizmos.DrawLine(pivot, transform.position);

        // Trazo circular completo
        if (mode == MotionMode.FullCircle)
        {
            switch (rotationPlane)
            {
                case RotationPlane.XZ:
                    UnityEditor.Handles.color = Color.cyan;
                    UnityEditor.Handles.DrawWireDisc(pivot, Vector3.up, radius);
                    break;
                case RotationPlane.XY:
                    UnityEditor.Handles.color = Color.cyan;
                    UnityEditor.Handles.DrawWireDisc(pivot, Vector3.forward, radius);
                    break;
                case RotationPlane.YZ:
                    UnityEditor.Handles.color = Color.cyan;
                    UnityEditor.Handles.DrawWireDisc(pivot, Vector3.right, radius);
                    break;
            }
        }
    }
#endif
}
