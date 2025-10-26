using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;             // Prędkość poruszania się wroga
    private bool movingRight = true;         // Kierunek ruchu

    [Header("Ground Detection")]
    public Transform groundDetection;        // Punkt wykrywający grunt
    public float detectionDistance = 2f;     // Długość raycastu
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
        // Poruszanie w poziomie
        rb.linearVelocity = new Vector2(moveSpeed * (movingRight ? 1 : -1), rb.linearVelocity.y);

        // Sprawdzanie przepaści
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, detectionDistance, groundLayer);
        if (groundInfo.collider == false)
        {
            Flip();
        }

        // Sprawdzanie ścian
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetection.position, movingRight ? Vector2.right : Vector2.left, wallDetectionDistance, groundLayer);
        if (wallInfo.collider == true)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Odwraca sprite
        transform.localScale = localScale;
    }

    // Opcjonalnie: podgląd raycastów w edytorze
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
