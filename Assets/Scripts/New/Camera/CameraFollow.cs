using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Camera Offset")]
    public Vector2 offset = new Vector2(2f, 1f);

    [Header("Smoothness")]
    public float smoothSpeed = 0.125f;

    [Header("Falling Settings")]
    public float fallingYOffset = -2f;
    public float fallThreshold = -1f;

    private Vector3 velocity = Vector3.zero;
    private Rigidbody2D targetRigidbody;

    private bool initialized = false;

    void Start()
{
    if (target != null)
    {
        targetRigidbody = target.GetComponent<Rigidbody2D>();
        initialized = true; // TO JEST KLUCZ
    }
}

    void FixedUpdate()
    {
        if (!initialized || target == null) return;

        float direction = Mathf.Sign(target.localScale.x);
        float yOffset = offset.y;

        if (targetRigidbody != null && targetRigidbody.linearVelocity.y < fallThreshold)
        {
            yOffset += fallingYOffset;
        }

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x * direction,
            target.position.y + yOffset,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothSpeed
        );
    }

    // wywoływane po respawnie
    public void SnapToTarget()
    {
        if (target == null) return;

        float direction = Mathf.Sign(target.localScale.x);
        float yOffset = offset.y;

        transform.position = new Vector3(
            target.position.x + offset.x * direction,
            target.position.y + yOffset,
            transform.position.z
        );

        velocity = Vector3.zero;
        initialized = true;
    }
}