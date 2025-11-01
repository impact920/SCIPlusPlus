using UnityEngine;

public class Walker : MonoBehaviour
{
    public float speed = 2f;
    public Transform groundCheck;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private int direction = 1; // 1 = w prawo, -1 = w lewo

    void Update()
    {
        // przesuwanie postaci
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Sprawdzenie przeszkody przed sobą
        Vector2 frontPos = new Vector2(transform.position.x + direction * 0.5f, transform.position.y);
        RaycastHit2D wallHit = Physics2D.Raycast(frontPos, Vector2.right * direction, 0.1f, groundLayer);
        Debug.DrawRay(frontPos, Vector2.right * direction * 0.1f, Color.red);

        // Sprawdzenie przepaści
        RaycastHit2D groundHit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(groundCheck.position, Vector2.down * groundCheckDistance, Color.green);

        if (wallHit.collider != null || groundHit.collider == null)
        {
            Flip();
        }
    }

    void Flip()
    {
        direction *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
