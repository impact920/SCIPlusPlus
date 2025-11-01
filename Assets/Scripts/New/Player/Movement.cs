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

    // Duch teleportacyjny (Q)
    [Header("Duch Teleportacyjny (Q)")]
    public GameObject ghostPrefab;
    public float ghostMoveSpeedMultiplier = 1.5f;
    public float ghostJumpMultiplier = 1.5f;
    public float ghostDashMultiplier = 1.2f;
    public float ghostLifetime = 3f;
    public float ghostCooldown = 5f;
    private bool ghostActive;
    private bool canUseGhost = true;
    private GameObject activeGhost;

    // Duch cofający (F)
    [Header("Duch Cofający (F)")]
    public GameObject staticGhostPrefab;
    public float staticGhostLifetime = 5f;
    public float staticGhostCooldown = 6f;
    public float positionRecordInterval = 0.2f;
    private List<Vector3> positionHistory = new List<Vector3>();
    private float recordTimer;
    private bool canUseStaticGhost = true;
    private bool staticGhostActive;

    private bool abilityInUse = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Rejestrowanie pozycji (dla cofania)
        recordTimer += Time.deltaTime;
        if (recordTimer >= positionRecordInterval)
        {
            recordTimer = 0f;
            positionHistory.Add(transform.position);
            if (positionHistory.Count > 10)
                positionHistory.RemoveAt(0);
        }

        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!wasGrounded && isGrounded)
            jumpPerformed = false;

        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

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

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isDashing && !jumpPerformed)
        {
            Jump();
        }

        if (jumpButtonHeld)
        {
            jumpHoldTimer += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.Q) && canUseGhost && !abilityInUse)
        {
            StartCoroutine(SpawnGhost());
        }

        if (Input.GetKeyDown(KeyCode.F) && canUseStaticGhost && !abilityInUse)
        {
            StartCoroutine(SpawnStaticGhostAndRewind());
        }

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

    // ✅ Poprawiony Duch teleportacyjny (Q) — bliżej gracza
    private IEnumerator SpawnGhost()
    {
        abilityInUse = true;
        canUseGhost = false;
        ghostActive = true;

        float direction = Mathf.Sign(transform.localScale.x);
        float spawnDistance = 0.6f; // 👈 teraz duch pojawia się tuż obok gracza

        // 🔍 Raycast aby sprawdzić, czy przed graczem jest ściana
        Vector2 origin = transform.position;
        RaycastHit2D hitFront = Physics2D.Raycast(origin, Vector2.right * direction, spawnDistance, groundLayer);

        Vector3 spawnPosition;

        if (hitFront.collider != null)
        {
            // 🔄 Ściana z przodu – spawn z tyłu gracza
            spawnPosition = transform.position - new Vector3(direction * spawnDistance, 0f, 0f);
        }
        else
        {
            // ✅ Brak przeszkody – spawn z przodu gracza
            spawnPosition = transform.position + new Vector3(direction * spawnDistance, 0f, 0f);
        }

        // 🧱 Jeśli spawn w kolizji, przerzuć stronę
        Collider2D overlap = Physics2D.OverlapCircle(spawnPosition, 0.25f, groundLayer);
        if (overlap != null)
        {
            spawnPosition = transform.position - new Vector3(direction * spawnDistance, 0f, 0f);
        }

        // Tworzymy ducha
        activeGhost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
        activeGhost.transform.localScale = new Vector3(direction, 1f, 1f);

        // Fade-in efekt
        SpriteRenderer sr = activeGhost.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;

            float fadeDuration = 0.3f;
            float timer = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                sr.color = c;
                yield return null;
            }
        }

        // Parametry ducha
        PlayerGhost ghostScript = activeGhost.GetComponent<PlayerGhost>();
        ghostScript.followInput = () => moveInput;
        ghostScript.jumpInput = () => Input.GetButtonDown("Jump");
        ghostScript.jumpForce = jumpForce * ghostJumpMultiplier;
        ghostScript.moveSpeed = moveSpeed * ghostMoveSpeedMultiplier;
        ghostScript.groundLayer = groundLayer;
        ghostScript.groundCheckRadius = groundCheckRadius;

        // Czekamy określony czas
        yield return new WaitForSeconds(ghostLifetime);

        if (activeGhost != null)
        {
            Vector3 startPos = transform.position;
            transform.position = activeGhost.transform.position;

            // Efekt smugi
            GameObject trail = new GameObject("TeleportTrail");
            trail.transform.position = startPos;

            TrailRenderer tr = trail.AddComponent<TrailRenderer>();
            tr.time = 0.3f;
            tr.startWidth = 0.2f;
            tr.endWidth = 0f;
            tr.material = new Material(Shader.Find("Sprites/Default"));
            tr.startColor = Color.cyan;
            tr.endColor = Color.clear;

            float duration = 0.1f;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                tr.transform.position = Vector3.Lerp(startPos, transform.position, t / duration);
                yield return null;
            }

            Destroy(trail);
            Destroy(activeGhost);
        }

        ghostActive = false;
        abilityInUse = false;

        yield return new WaitForSeconds(ghostCooldown);
        canUseGhost = true;
    }

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

        GameObject ghost = Instantiate(staticGhostPrefab, transform.position, Quaternion.identity);
        ghost.tag = "StaticGhost";

        Vector3 rewindPosition = positionHistory[0];
        Vector3 startPos = transform.position;

        transform.position = rewindPosition;

        GameObject trail = new GameObject("RewindTrail");
        trail.transform.position = startPos;

        TrailRenderer tr = trail.AddComponent<TrailRenderer>();
        tr.time = 0.3f;
        tr.startWidth = 0.2f;
        tr.endWidth = 0f;
        tr.material = new Material(Shader.Find("Sprites/Default"));
        tr.startColor = Color.red;
        tr.endColor = Color.clear;

        float duration = 0.3f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            tr.transform.position = Vector3.Lerp(startPos, transform.position, t / duration);
            yield return null;
        }

        Destroy(trail);

        yield return new WaitForSeconds(staticGhostLifetime);
        if (ghost != null) Destroy(ghost);

        staticGhostActive = false;
        abilityInUse = false;

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
