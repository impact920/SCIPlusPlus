using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float slideSpeed = 10f;
    public float jumpForce = 10f;
    public float doubleJumpForce = 10f;
    public float dashDistance = 15f;
    public float dashCooldown = 1f;

    [Header("Slide Settings")]
    public float slideCooldown = 1f; // Cooldown wślizgu

    [Header("Slow Surface Settings")]
    public string slowSurfaceTag = "SlowSurface";
    public LayerMask slowSurfaceLayer;
    public float slowWalkSpeed = 2.5f;
    public float slowJumpForce = 5f;
    public float slowDoubleJumpForce = 5f;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaUsagePerSecond = 20f;
    public float staminaRegenPerSecond = 10f;
    public float staminaUsagePerJump = 15f;
    public Image staminaBar;

    [Header("Physics")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool isDashing;
    private bool canDash = true;
    private bool canSlide = true; // Czy wślizg jest dostępny

    private PlayerAttacking PlayerAttacking;
    private float horizontalInput;
    private bool isSliding;
    private float currentStamina;
    private bool isOnSlowSurface;

    private float currentWalkSpeed;
    private float currentJumpForce;
    private float currentDoubleJumpForce;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentStamina = maxStamina;

        currentWalkSpeed = walkSpeed;
        currentJumpForce = jumpForce;
        currentDoubleJumpForce = doubleJumpForce;

        PlayerAttacking = GetComponent<PlayerAttacking>();

        UpdateStaminaUI();
    }

    void Update()
    {
        if (isDashing) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded) canDoubleJump = true;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;
        float speed = isRunning ? runSpeed : currentWalkSpeed;
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);

        animator.SetBool("IsRunning", isRunning);

        if (isRunning)
        {
            UseStamina(staminaUsagePerSecond * Time.deltaTime);
        }
        else
        {
            RegenerateStamina(staminaRegenPerSecond * Time.deltaTime);
        }

        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", isGrounded);

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded && currentStamina >= staminaUsagePerJump)
            {
                Jump(currentJumpForce);
                UseStamina(staminaUsagePerJump);
            }
            else if (canDoubleJump && currentStamina >= staminaUsagePerJump)
            {
                Jump(currentDoubleJumpForce);
                UseStamina(staminaUsagePerJump);
                canDoubleJump = false;
                animator.SetTrigger("DoubleJump");
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && canSlide && isGrounded && horizontalInput != 0)
        {
            UseStamina(20f);
            StartCoroutine(Slide());
        }

        if (Input.GetKeyDown(KeyCode.E) && canDash && currentStamina >= 20f)
        {
            UseStamina(50f);
            StartCoroutine(Dash());
        }
    }

    private void Jump(float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        animator.SetTrigger("Jump");
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        canSlide = false; // Zablokuj możliwość wślizgu podczas cooldownu
        float originalSpeed = currentWalkSpeed;
        currentWalkSpeed = slideSpeed;

        animator.SetTrigger("Slide");

        yield return new WaitForSeconds(0.5f); // Czas trwania wślizgu

        currentWalkSpeed = originalSpeed;
        isSliding = false;

        yield return new WaitForSeconds(slideCooldown); // Czas cooldownu
        canSlide = true; // Odblokuj wślizg
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;

        Vector2 dashDirection = new Vector2(horizontalInput, 0).normalized;
        if (dashDirection == Vector2.zero)
            dashDirection = Vector2.right * transform.localScale.x;

        rb.linearVelocity = dashDirection * dashDistance;

        animator.SetTrigger("Dash");

        PlayerAttacking.ActivateImmortality();

        yield return new WaitForSeconds(0.2f);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }

    private void RegenerateStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        staminaBar.fillAmount = currentStamina / maxStamina;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(slowSurfaceTag) ||
            (slowSurfaceLayer != 0 && ((1 << collision.collider.gameObject.layer) & slowSurfaceLayer) != 0))
        {
            isOnSlowSurface = true;
            ApplySlowSurfaceSettings();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(slowSurfaceTag) ||
            (slowSurfaceLayer != 0 && ((1 << collision.collider.gameObject.layer) & slowSurfaceLayer) != 0))
        {
            isOnSlowSurface = false;
            ResetToNormalSettings();
        }
    }

    private void ApplySlowSurfaceSettings()
    {
        currentWalkSpeed = slowWalkSpeed;
        currentJumpForce = slowJumpForce;
        currentDoubleJumpForce = slowDoubleJumpForce;
    }

    private void ResetToNormalSettings()
    {
        currentWalkSpeed = walkSpeed;
        currentJumpForce = jumpForce;
        currentDoubleJumpForce = doubleJumpForce;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
