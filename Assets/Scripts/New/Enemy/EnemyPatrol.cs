using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;             
    private bool movingRight = true;         // Kierunek ruchu

    [Header("Ground Detection")]
    public Transform groundDetection;        
    public float detectionDistance = 2f;     // Długość raycastu
    public LayerMask groundLayer;            // Warstwa ziemi

    [Header("Wall Detection")]
    public Transform wallDetection;          // Punkt wykrywający ściany
    public float wallDetectionDistance = 0.5f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool wasGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Podstawowy ruch 
        rb.linearVelocity = new Vector2(moveSpeed * (movingRight ? 1 : -1), rb.linearVelocity.y);

        // Sprawdzanie ziemi
        wasGrounded = isGrounded;
        isGrounded = Physics2D.Raycast(groundDetection.position, Vector2.down, detectionDistance, groundLayer);

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

        // ANTI-STEP FIX jak u gracza
        // Jeśli wróg jest na ziemi i porusza się, sprawdzamy mikroszczeliny pod nogami
        if (isGrounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.3f, groundLayer);

            if (hit.collider != null && hit.normal.y < 0.99f && hit.distance < 0.1f)
            {
                // Delikatnie unosimy pozycję, by wróg "wjechał" na próg zamiast się zablokować
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0.1f);
            }
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Odwraca sprite
        transform.localScale = localScale;
    }

    // Podgląd w edytorze
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
