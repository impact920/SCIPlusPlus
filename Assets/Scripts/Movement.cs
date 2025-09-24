using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    Rigidbody2D rb;
    bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Ruch poziomy
        float move = Input.GetAxisRaw("Horizontal"); // strzałki ← → lub A/D
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);

        // Sprawdzanie czy dotykamy ziemi
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Skok
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
