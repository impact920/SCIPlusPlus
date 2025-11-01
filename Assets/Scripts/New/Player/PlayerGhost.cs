using UnityEngine;
using System.Collections;

public class PlayerGhost : MonoBehaviour
{
    public float moveSpeed = 12f;
    public float jumpForce = 18f;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    public Transform groundCheck;

    public System.Func<float> followInput;
    public System.Func<bool> jumpInput;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Stwórz groundCheck jeśli nie istnieje
        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GhostGroundCheck");
            gc.transform.parent = transform;
            gc.transform.localPosition = Vector3.down * 0.5f;
            groundCheck = gc.transform;
        }
    }

    private void Update()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        if (jumpInput != null && jumpInput.Invoke() && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        if (followInput != null)
        {
            float move = followInput.Invoke();
            rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        }
    }

    public void StartDash(float direction, float dashForce, float dashDuration)
    {
        StartCoroutine(DashRoutine(direction, dashForce, dashDuration));
    }

    private IEnumerator DashRoutine(float direction, float dashForce, float dashDuration)
    {
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(direction * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
