using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class ObjectMover2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public List<Transform> waypoints = new List<Transform>();
    public float baseSpeed = 2f;
    public float pauseDuration = 1f;
    public bool loop = true;
    public bool pingPong = false;

    [Header("Easing Settings")]
    public EasingType easing = EasingType.Linear;

    [Header("Debug Settings")]
    public Vector2 currentVelocity; // Public to allow other scripts to read

    private Rigidbody2D rb;
    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private Vector2 previousPosition;
    private Coroutine movementCoroutine;
    private float currentSpeed;

    void Start()
    {
        if (waypoints.Count < 2)
        {
            Debug.LogError("ObjectMover2D requires at least two waypoints.");
            enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        previousPosition = rb.position;
        currentSpeed = baseSpeed;
        movementCoroutine = StartCoroutine(ExecuteMovement());
    }

    IEnumerator ExecuteMovement()
    {
        while (true)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector2 startPosition = rb.position;
            Vector2 targetPosition = targetWaypoint.position;
            Vector2 direction = (targetPosition - startPosition).normalized;
            float distance = Vector2.Distance(startPosition, targetPosition);
            float duration = distance / currentSpeed;

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                float easedT = ApplyEasing(t, easing);
                Vector2 newPosition = Vector2.Lerp(startPosition, targetPosition, easedT);
                rb.MovePosition(newPosition);

                // Calculate velocity manually based on position change
                currentVelocity = (rb.position - previousPosition) / Time.deltaTime;
                previousPosition = rb.position;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rb.MovePosition(targetPosition);
            currentVelocity = Vector2.zero;
            yield return new WaitForSeconds(pauseDuration);

            // Determine next waypoint index
            if (pingPong)
            {
                movingForward = !movingForward;
                if (movingForward)
                {
                    currentWaypointIndex++;
                    if (currentWaypointIndex >= waypoints.Count)
                        currentWaypointIndex = waypoints.Count - 2;
                }
                else
                {
                    currentWaypointIndex--;
                    if (currentWaypointIndex < 0)
                        currentWaypointIndex = 1;
                }
            }
            else
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Count)
                {
                    if (loop)
                        currentWaypointIndex = 0;
                    else
                        currentWaypointIndex = waypoints.Count - 1;
                }
            }
        }
    }

    private float ApplyEasing(float t, EasingType easing)
    {
        switch (easing)
        {
            case EasingType.Linear:
                return t;
            case EasingType.EaseIn:
                return t * t;
            case EasingType.EaseOut:
                return t * (2 - t);
            case EasingType.EaseInOut:
                return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
            default:
                return t;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Set player as child when it collides with the platform
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
            Debug.Log("Player parented to platform");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Remove player from platform when collision ends
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
            Debug.Log("Player unparented from platform");
        }
    }

    public void SetBaseSpeed(float newSpeed)
    {
        baseSpeed = newSpeed;
        currentSpeed = baseSpeed;
    }

    public void AdjustSpeed(float multiplier)
    {
        currentSpeed = baseSpeed * multiplier;
    }

    public void ResetSpeed()
    {
        currentSpeed = baseSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Count < 2)
            return;

        Gizmos.color = Color.green;
        Vector3 startPoint = transform.position;

        foreach (var waypoint in waypoints)
        {
            if (waypoint == null)
                continue;

            Vector3 endPoint = waypoint.position;
            Gizmos.DrawLine(startPoint, endPoint);
            DrawArrow(startPoint, endPoint, Color.green);
            Gizmos.DrawSphere(endPoint, 0.1f);

#if UNITY_EDITOR
            Handles.Label(endPoint + Vector3.up * 0.2f, waypoint.name);
#endif
            startPoint = endPoint;
        }

        if (loop && waypoints.Count > 1)
        {
            Vector3 firstPoint = waypoints[0].position;
            Gizmos.DrawLine(startPoint, firstPoint);
            DrawArrow(startPoint, firstPoint, Color.green);
            Gizmos.DrawSphere(firstPoint, 0.1f);
        }
    }

    private void DrawArrow(Vector3 start, Vector3 end, Color color)
    {
        Gizmos.color = color;
        Vector3 direction = (end - start).normalized;
        float arrowHeadLength = 0.2f;
        float arrowHeadAngle = 20f;

        Gizmos.DrawLine(start, end);

        Vector3 right = Quaternion.Euler(0, 0, -arrowHeadAngle) * direction;
        Vector3 left = Quaternion.Euler(0, 0, arrowHeadAngle) * direction;

        Gizmos.DrawLine(end, end - right * arrowHeadLength);
        Gizmos.DrawLine(end, end - left * arrowHeadLength);
    }

    public enum EasingType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }
}
