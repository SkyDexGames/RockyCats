using UnityEngine;

public class PathPlatform : PlatformMovement
{
    [SerializeField] private Transform[] points;
    private int current = 0;

    protected override void Move()
    {
        if (points.Length == 0) return;

        transform.position = Vector3.MoveTowards(transform.position, points[current].position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, points[current].position) < 0.1f)
        {
            current++;
            if (current >= points.Length)
                current = loop ? 0 : points.Length - 1;
        }
    }
}
