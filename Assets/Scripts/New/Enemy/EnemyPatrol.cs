using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;             // Prędkość poruszania się wroga
    private bool movingRight = true;         // Kierunek ruchu

    [Header("Ground Detection")]
    public Transform groundDetection;        // Punkt wykrywający grunt (na przodzie nóg)
    public float detectionDistance = 1.5f;   // Długość raycastu w dół
    public LayerMask groundLayer;            // Warstwa ziemi

    [Header("Wall Detection")]
    public Transform wallDetection;          // Punkt wykrywający ściany
    public float wallDetectionDistance = 0.5f; 

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // --- Ruch poziomy ---
        rb.linearVelocity = new Vector2(moveSpeed * (movingRight ? 1 : -1), rb.linearVelocity.y);

        // --- Wykrywanie przepaści przed wrogiem ---
        RaycastHit2D groundInfo = Physics2D.Raycast(
            groundDetection.position,
            Vector2.down,
            detectionDistance,
            groundLayer
        );

        // 🔍 Rysowanie pomocnicze w edytorze (żeby widzieć raycast)
        Debug.DrawLine(groundDetection.position, groundDetection.position + Vector3.down * detectionDistance, Color.red);

        if (!groundInfo.collider)
        {
            // ⚠️ Brak gruntu przed wrogiem — zawróć
            Flip();
            return;
        }

        // --- Wykrywanie ścian przed wrogiem ---
        Vector2 wallDir = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D wallInfo = Physics2D.Raycast(
            wallDetection.position,
            wallDir,
            wallDetectionDistance,
            groundLayer
        );

        Debug.DrawLine(wallDetection.position, wallDetection.position + (Vector3)wallDir * wallDetectionDistance, Color.blue);

        if (wallInfo.collider)
        {
            // ⚠️ Ściana — zawróć
            Flip();
            return;
        }

        // --- Anti-step fix (podnoszenie przy małych nierównościach) ---
        RaycastHit2D stepHit = Physics2D.Raycast(transform.position, Vector2.down, 0.3f, groundLayer);

        if (stepHit.collider != null && stepHit.normal.y < 0.99f && stepHit.distance < 0.1f)
        {
            rb.position += Vector2.up * 0.05f;
        }

        if (!groundInfo.collider)
{
    Debug.Log("Brak gruntu — odwracam się!");
    Flip();
    return;
}

    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void OnDrawGizmosSelected()
    {
        if (groundDetection != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundDetection.position, groundDetection.position + Vector3.down * detectionDistance);
        }

        if (wallDetection != null)
        {
            Gizmos.color = Color.blue;
            Vector3 direction = movingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallDetection.position, wallDetection.position + direction * wallDetectionDistance);
        }
    }
}
