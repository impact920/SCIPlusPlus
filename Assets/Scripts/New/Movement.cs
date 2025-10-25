using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ruch")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Skok (zmienna wysokość)")]
    public float lowJumpMultiplier = 2f;
    public float minHoldTime = 0.15f;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Coyote Time")]
    public float coyoteTime = 0.5f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer")]
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

    private bool jumpButtonHeld;
    private float jumpHoldTimer;

    // 🔮 Duch teleportacyjny (Q)
    [Header("Duch Teleportacyjny (Q)")]
    public GameObject ghostPrefab;
    public float ghostMoveSpeedMultiplier = 1.5f;
    public float ghostJumpMultiplier = 1.5f;
    public float ghostDashMultiplier = 1.2f;
    public float ghostLifetime = 3f;
    public float ghostCooldown = 5f; // ⏳ Cooldown Q
    private bool ghostActive;
    private bool canUseGhost = true;
    private GameObject activeGhost;

    // ⏳ Duch cofający (F)
    [Header("Duch Cofający (F)")]
    public GameObject staticGhostPrefab;
    public float staticGhostLifetime = 5f;
    public float staticGhostCooldown = 6f; // ⏳ Cooldown F
    public float positionRecordInterval = 0.2f;
    private List<Vector3> positionHistory = new List<Vector3>();
    private float recordTimer;
    private bool canUseStaticGhost = true;
    private bool staticGhostActive;

    // 🧩 Flaga blokująca jednoczesne użycie duchów
    private bool abilityInUse = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // 🔁 Rejestrowanie pozycji co 0.2s (dla cofania)
        recordTimer += Time.deltaTime;
        if (recordTimer >= positionRecordInterval)
        {
            recordTimer = 0f;
            positionHistory.Add(transform.position);
            if (positionHistory.Count > 15)
                positionHistory.RemoveAt(0);
        }

        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!wasGrounded && isGrounded)
            jumpPerformed = false;

        // Coyote time
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // Jump buffer
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

        // Skok
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isDashing && !jumpPerformed)
        {
            Jump();
        }

        if (jumpButtonHeld)
        {
            jumpHoldTimer += Time.deltaTime;
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(Dash());
        }

        // 🔮 Duch teleportacyjny (Q)
        if (Input.GetKeyDown(KeyCode.Q) && canUseGhost && !abilityInUse)
        {
            StartCoroutine(SpawnGhost());
        }

        // ⏳ Duch cofający (F)
        if (Input.GetKeyDown(KeyCode.F) && canUseStaticGhost && !abilityInUse)
        {
            StartCoroutine(SpawnStaticGhostAndRewind());
        }

        // Skrócony skok
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

    private IEnumerator Dash()
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

    // 🔮 Duch teleportacyjny (Q)
    private IEnumerator SpawnGhost()
    {
        abilityInUse = true;
        canUseGhost = false;
        ghostActive = true;

        activeGhost = Instantiate(ghostPrefab, transform.position, Quaternion.identity);
        PlayerGhost ghostScript = activeGhost.GetComponent<PlayerGhost>();

        ghostScript.followInput = () => moveInput;
        ghostScript.jumpInput = () => Input.GetButtonDown("Jump");
        ghostScript.jumpForce = jumpForce * ghostJumpMultiplier;
        ghostScript.moveSpeed = moveSpeed * ghostMoveSpeedMultiplier;
        ghostScript.groundLayer = groundLayer;
        ghostScript.groundCheckRadius = groundCheckRadius;

        yield return new WaitForSeconds(ghostLifetime);

        if (activeGhost != null)
        {
            transform.position = activeGhost.transform.position;
            Destroy(activeGhost);
        }

        ghostActive = false;
        abilityInUse = false;

        // Cooldown Q
        yield return new WaitForSeconds(ghostCooldown);
        canUseGhost = true;
    }

    // ⏳ Duch cofający (F)
    private IEnumerator SpawnStaticGhostAndRewind()
    {
        abilityInUse = true;
        canUseStaticGhost = false;
        staticGhostActive = true;

        if (positionHistory.Count < 2)
        {
            staticGhostActive = false;
            abilityInUse = false;
            yield break;
        }

        // 1️⃣ Postaw ducha w obecnej pozycji
        GameObject ghost = Instantiate(staticGhostPrefab, transform.position, Quaternion.identity);
        ghost.tag = "StaticGhost";

        // 2️⃣ Cofnij gracza do pozycji sprzed ~3 sekund
        Vector3 rewindPosition = positionHistory[0];
        transform.position = rewindPosition;

        // 3️⃣ Poczekaj 5 sekund i usuń ducha
        yield return new WaitForSeconds(staticGhostLifetime);
        if (ghost != null) Destroy(ghost);

        staticGhostActive = false;
        abilityInUse = false;

        // Cooldown F
        yield return new WaitForSeconds(staticGhostCooldown);
        canUseStaticGhost = true;
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
