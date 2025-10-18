using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ruch")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Skok (zmienna wysokość)")]
    [Tooltip("Jak bardzo zwiększyć grawitację po puszczeniu spacji")]
    public float lowJumpMultiplier = 2f;
    [Tooltip("Jak długo trzeba trzymać spację, żeby uzyskać maksymalny skok (sekundy)")]
    public float minHoldTime = 0.15f;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Coyote Time (czas po zejściu z platformy)")]
    public float coyoteTime = 0.5f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer (czas bufora skoku)")]
    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Kontrola gruntu")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isDashing;
    private bool canDash = true;
    private bool jumpPerformed;

    // Dodatkowe do „krótkiego przytrzymania spacji”
    private bool jumpButtonHeld;
    private float jumpHoldTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!wasGrounded && isGrounded)
            jumpPerformed = false;

        // Liczenie coyote time
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Bufor skoku
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
            jumpButtonHeld = true;
            jumpHoldTimer = 0f;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            jumpButtonHeld = false;
        }

        jumpBufferCounter -= Time.deltaTime;

        // Skok (wykonuje się raz)
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isDashing && !jumpPerformed)
        {
            Jump();
        }

        // Liczenie czasu przytrzymania spacji
        if (jumpButtonHeld)
        {
            jumpHoldTimer += Time.deltaTime;
        }

        // DASH pod klawiszem E
        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(Dash());
        }

        // Jeśli gracz puścił spację WCZEŚNIEJ niż po minHoldTime i wciąż się wznosi — skróć skok
        if (rb.linearVelocity.y > 0 && !jumpButtonHeld && jumpHoldTimer < minHoldTime)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
        jumpPerformed = true;
        jumpHoldTimer = 0f;
        jumpButtonHeld = true;
    }

    private System.Collections.IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = moveInput != 0 ? moveInput : Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(dashDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
