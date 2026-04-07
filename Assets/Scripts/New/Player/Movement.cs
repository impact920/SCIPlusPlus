using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    [Header("Jump")]
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

    [Header("Ground Check")]
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

    [Header("Teleport Ghost (Q)")]
    public GameObject ghostPrefab;
    public float ghostMoveSpeedMultiplier = 1.5f;
    public float ghostJumpMultiplier = 1.5f;
    public float ghostDashMultiplier = 1.2f;
    public float ghostLifetime = 3f;
    public float ghostCooldown = 5f;
    private bool ghostActive;
    private bool canUseGhost = true;
    private GameObject activeGhost;

    [Header("Revind Ghost (F)")]
    public GameObject staticGhostPrefab;
    public float staticGhostLifetime = 5f;
    public float staticGhostCooldown = 6f;
    public float positionRecordInterval = 0.2f;
    private List<Vector3> positionHistory = new List<Vector3>();
    private float recordTimer;
    private bool canUseStaticGhost = true;
    private bool staticGhostActive;
    private bool abilityInUse = false;
    // --- ANIMATOR ---
    private Animator anim;

    [Header("Attack")]

    private bool isAttacking = false;

    [Header("Attack Movement Modifier")]
    [Range(0f, 1f)]
    public float attackMoveSpeedMultiplier = 0.5f;

    [Header("Attack Buffer")]
public float attackBufferTime = 0.2f;
private float attackBufferCounter = 0f;

private float attackFacingDirection;

private SpriteRenderer sr;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Rejestrowanie pozycji dla cofania
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

        // reset jump
        if (!wasGrounded && isGrounded)
            jumpPerformed = false;

        // coyote time
        if (isGrounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        // jump buffer
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

        // DASH
        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(Dash());
        }

        // GHOST TELEPORT Q
        if (Input.GetKeyDown(KeyCode.Q) && canUseGhost && !abilityInUse)
        {
            StartCoroutine(SpawnGhost());
        }

        // STATIC GHOST F
        if (Input.GetKeyDown(KeyCode.F) && canUseStaticGhost && !abilityInUse)
        {
            StartCoroutine(SpawnStaticGhostAndRewind());
        }

        // Zmienna wysokość skoku
        if (rb.velocity.y > 0 && !jumpButtonHeld && jumpHoldTimer < minHoldTime)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // Obracanie gracza
        if (!isAttacking)
{
    if (moveInput != 0)
    {
        sr.flipX = moveInput < 0;
    }
}
else
{
    sr.flipX = attackFacingDirection < 0;
}



        // Animator
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsMoving", Mathf.Abs(moveInput) > 0.1f);
        anim.SetBool("IsJumping", rb.velocity.y > 0.1f && !isGrounded);
        anim.SetBool("IsFalling", rb.velocity.y < -0.1f && !isGrounded);
        anim.SetBool("IsDashing", isDashing);

        // Atak
        if (Input.GetMouseButtonDown(0))
{
    attackBufferCounter = attackBufferTime;
}

// zmniejszanie bufora
attackBufferCounter -= Time.deltaTime;

// jeśli można atakować i jest klik w buforze
if (attackBufferCounter > 0f && !isAttacking && !isDashing && !abilityInUse)
{
    StartRandomAttack();
    attackBufferCounter = 0f;
}

    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            float currentSpeed = moveSpeed;

            if (isAttacking)
                currentSpeed *= attackMoveSpeedMultiplier;
                rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
            
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
        jumpPerformed = true;
        jumpHoldTimer = 0f;
        jumpButtonHeld = true;

        if (activeGhost != null)
        {
            PlayerGhost ghostScript = activeGhost.GetComponent<PlayerGhost>();
            if (ghostScript != null)
                ghostScript.TriggerJump();
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        anim.SetTrigger("Dash");

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = moveInput != 0 ? moveInput : Mathf.Sign(transform.localScale.x);
        rb.velocity = new Vector2(dashDirection * dashForce, 0f);

        if (activeGhost != null)
        {
            PlayerGhost ghostScript = activeGhost.GetComponent<PlayerGhost>();
            if (ghostScript != null)
            {
                ghostScript.StartDash(dashDirection, dashForce * ghostDashMultiplier, dashDuration);
            }
        }

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public bool IsDashing()
{
    return isDashing;
}


    private void StartRandomAttack()
    {

        anim.ResetTrigger("Attack1");
        anim.ResetTrigger("Attack2");
        anim.ResetTrigger("Attack3");

        if (isAttacking) return;

        isAttacking = true;
        attackFacingDirection = sr.flipX ? -1f : 1f;

        int randomAttack = Random.Range(1, 3); // 1–3

        switch (randomAttack)
        {
            case 1:
            anim.SetTrigger("Attack1");
            break;

            case 2:
            anim.SetTrigger("Attack2");
            break;

            case 3:
            anim.SetTrigger("Attack3");
            break;
        }
    }

    public void OnAttackEnd()
    {   
        isAttacking = false;
    }



    

    private IEnumerator SpawnGhost()
    {
        anim.SetBool("SkillMode", true);
        abilityInUse = true;
        canUseGhost = false;
        ghostActive = true;

        float direction = Mathf.Sign(transform.localScale.x);
        float spawnDistance = 0.6f;

        Vector3 spawnPosition = transform.position + new Vector3(direction * spawnDistance, 0f, 0f);
        activeGhost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
        activeGhost.transform.localScale = new Vector3(direction, 1f, 1f);

        PlayerGhost ghostScript = activeGhost.GetComponent<PlayerGhost>();
        ghostScript.moveSpeed = moveSpeed * ghostMoveSpeedMultiplier;
        ghostScript.jumpForce = jumpForce * ghostJumpMultiplier;
        ghostScript.groundLayer = groundLayer;
        ghostScript.groundCheckRadius = groundCheckRadius;

        StartCoroutine(SyncGhostWithPlayer(ghostScript));

        yield return new WaitForSeconds(ghostLifetime);

        if (activeGhost != null)
        {
            transform.position = activeGhost.transform.position;
            Destroy(activeGhost);
        }

        ghostActive = false;
        abilityInUse = false;

        anim.SetBool("SkillMode", false);

        yield return new WaitForSeconds(ghostCooldown);
        canUseGhost = true;
    }

    private IEnumerator SyncGhostWithPlayer(PlayerGhost ghost)
    {
        bool lastFacingRight = transform.localScale.x > 0;

        while (ghost != null)
        {
            bool facingRight = transform.localScale.x > 0;
            if (facingRight != lastFacingRight)
            {
                ghost.transform.localScale = new Vector3(transform.localScale.x, 1f, 1f);
                lastFacingRight = facingRight;
            }

            ghost.SetMoveInput(moveInput);

            if (Input.GetKeyDown(KeyCode.E) && canDash)
            {
                float direction = moveInput != 0 ? moveInput : Mathf.Sign(transform.localScale.x);
                ghost.StartDash(direction, dashForce * ghostDashMultiplier, dashDuration);
            }

            yield return null;
        }
    }

    private IEnumerator SpawnStaticGhostAndRewind()
    {
        abilityInUse = true;
        canUseStaticGhost = false;
        staticGhostActive = true;

        anim.SetBool("SkillMode", true);

        if (positionHistory.Count < 2)
        {
            staticGhostActive = false;
            abilityInUse = false;
            anim.SetBool("SkillMode", false);
            yield break;
        }

        GameObject ghost = Instantiate(staticGhostPrefab, transform.position, Quaternion.identity);
        ghost.tag = "StaticGhost";

        Vector3 rewindPosition = positionHistory[0];

        transform.position = rewindPosition;

        yield return new WaitForSeconds(staticGhostLifetime);
        if (ghost != null) Destroy(ghost);

        staticGhostActive = false;
        abilityInUse = false;

        anim.SetBool("SkillMode", false);

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
