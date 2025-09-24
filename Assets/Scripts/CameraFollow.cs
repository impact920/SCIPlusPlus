using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The player character

    [Header("Camera Offset")]
    public Vector2 offset = new Vector2(2f, 1f); // Offset relative to the player

    [Header("Smoothness")]
    public float smoothSpeed = 0.125f; // How quickly the camera follows

    [Header("Falling Settings")]
    public float fallingYOffset = -2f; // Additional offset when falling
    public float fallThreshold = -1f; // Threshold for detecting falling (velocity.y)

    private Vector3 velocity = Vector3.zero;
    private Rigidbody2D targetRigidbody;

    void Start()
    {
        // Attempt to get the Rigidbody2D from the target
        if (target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody2D>();
            if (targetRigidbody == null)
            {
                Debug.LogWarning("Target does not have a Rigidbody2D component. Falling detection will not work.");
            }
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Determine the desired position of the camera based on the target's facing direction
        float direction = Mathf.Sign(target.localScale.x); // Determines if the player is facing left or right
        float yOffset = offset.y;

        // Check if the player is falling
        if (targetRigidbody != null && targetRigidbody.linearVelocity.y < fallThreshold)
        {
            yOffset += fallingYOffset;
        }

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x * direction,
            target.position.y + yOffset,
            transform.position.z
        );

        // Smoothly transition to the desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
    }
}
