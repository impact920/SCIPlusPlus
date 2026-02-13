using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // Gracz

    [Header("Camera Offset")]
    public Vector2 offset = new Vector2(2f, 1f); // Offset kamery

    [Header("Smoothness")]
    public float smoothSpeed = 0.125f; // Szybkosc

    [Header("Falling Settings")]
    public float fallingYOffset = -2f; // Dodatkowe przesuniecie podczas opadania
    public float fallThreshold = -1f; // Kiedy to sie zalacza

    private Vector3 velocity = Vector3.zero;
    private Rigidbody2D targetRigidbody;

    void Start()
    {
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

        // Okreslenie żzadanej pozycji kamery na podstawie kierunku, w którym skierowany jest gracz
        float direction = Mathf.Sign(target.localScale.x); // Okreslenie czy gracz patrzy w lewo czy w prawo
        float yOffset = offset.y;

        // Sprawdzanie czy gracz spada
        if (targetRigidbody != null && targetRigidbody.linearVelocity.y < fallThreshold)
        {
            yOffset += fallingYOffset;
        }

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x * direction,
            target.position.y + yOffset,
            transform.position.z
        );

        // Plynne przejscie do pozadanej pozycji
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
    }
}
