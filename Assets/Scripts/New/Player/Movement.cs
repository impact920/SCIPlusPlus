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

    [Header("Audio")]
    public AudioSource sfxSource; // skok, dash, atak
    public AudioSource footstepSource; // chodzenie

    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip[] footstepSounds;

    [Header("Attack Sounds")]
public AudioClip attackSound1;
public AudioClip attackSound2;

[Header("Charge Attack Sounds")]
public AudioClip chargeLoopSound;     // dźwięk ładowania
public AudioClip chargeReleaseSound;  // dźwięk release
private AudioSource chargeAudioSource;

[Header("Skill Sounds")]
public AudioClip ghostStartSound;
public AudioClip ghostEndSound;

public AudioClip rewindStartSound;
public AudioClip rewindEndSound;

[Header("Smooth Movement")]
public float acceleration = 70f;
public float deceleration = 60f;
public float velPower = 0.9f;

[Header("Air Control")]
public float airAccelerationMultiplier = 0.7f;
public float airDecelerationMultiplier = 0.5f;

[Header("Better Fall")]
public float fallMultiplier = 3.5f;

    private float footstepTimer;
    public float footstepInterval = 0.4f;

    [Header("Charged Dash Attack")]
public float maxChargeTime = 2f;
public float chargedDashForce = 35f;
public float chargedDashDuration = 0.4f;
public int minChargeDamage = 10;
public int maxChargeDamage = 50;

public LayerMask enemyLayer;

private bool isChargingAttack;
private float currentChargeTime;
public bool isInvulnerable;

[Header("Charge Movement")]
[Range(0f, 1f)]
public float chargeMoveSpeedMultiplier = 0.3f;
public float chargedAttackCooldown = 10f;

private bool canUseChargedAttack = true;


private List<EnemyHealth> hitEnemiesDuringCharge = new List<EnemyHealth>();

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        if (sfxSource == null || footstepSource == null)
        {
            Debug.LogWarning("Brak AudioSource na graczu!");
        }
        chargeAudioSource = gameObject.AddComponent<AudioSource>();
        chargeAudioSource.loop = true;
        chargeAudioSource.playOnAwake = false;
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
        if (Input.GetButtonDown("Dash") && canDash)
        {
            StartCoroutine(Dash());
        }

        // GHOST TELEPORT Q
        if (Input.GetButtonDown("Teleport Ghost") && canUseGhost && !abilityInUse)
        {
            StartCoroutine(SpawnGhost());
        }

        // STATIC GHOST F
        if (Input.GetButtonDown("Static Ghost") && canUseStaticGhost && !abilityInUse)
        {
            StartCoroutine(SpawnStaticGhostAndRewind());
        }

        // Zmienna wysokość skoku
        if (rb.linearVelocity.y > 0 && !jumpButtonHeld && jumpHoldTimer < minHoldTime)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
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
        anim.SetBool("IsJumping", rb.linearVelocity.y > 0.1f && !isGrounded);
        anim.SetBool("IsFalling", rb.linearVelocity.y < -0.1f && !isGrounded);
        anim.SetBool("IsDashing", isDashing);

        // Atak
        if (Input.GetButtonDown("Attack"))
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

        // FOOTSTEPS SYSTEM
        if (isGrounded && Mathf.Abs(moveInput) > 0.1f && !isDashing)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        // ŁADOWANIE ATAKU PPM
if (Input.GetButtonDown("Charged Attack")
    && !isChargingAttack
    && !isDashing
    && canUseChargedAttack)
{
    StartChargeAttack();
}

if (Input.GetButton("Charged Attack") && isChargingAttack)
{
    ChargeAttack();
}

if (Input.GetButtonUp("Charged Attack") && isChargingAttack)
{
    StartCoroutine(ReleaseChargeAttack());
}
    }

    private void FixedUpdate()
{
    if (isDashing) return;

    float targetSpeed = moveInput * moveSpeed;

    if (isAttacking)
    targetSpeed *= attackMoveSpeedMultiplier;

if (isChargingAttack)
    targetSpeed *= chargeMoveSpeedMultiplier;

    float accelRate;

    if (isGrounded)
    {
        accelRate = Mathf.Abs(targetSpeed) > 0.01f
            ? acceleration
            : deceleration;
    }
    else
    {
        accelRate = Mathf.Abs(targetSpeed) > 0.01f
            ? acceleration * airAccelerationMultiplier
            : deceleration * airDecelerationMultiplier;
    }

    float speedDif = targetSpeed - rb.linearVelocity.x;

    float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

    rb.AddForce(movement * Vector2.right);

    // LEPSZE SPADANIE
    if (rb.linearVelocity.y < 0)
    {
        rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
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

        sfxSource.PlayOneShot(jumpSound);

        if (activeGhost != null)
        {
            PlayerGhost ghostScript = activeGhost.GetComponent<PlayerGhost>();
            if (ghostScript != null)
                ghostScript.TriggerJump();
        }
    }

    private IEnumerator Dash()
{
    if (isChargingAttack)
    {
        CancelChargeAttack();
    }

    canDash = false;
    isDashing = true;

    anim.SetTrigger("Dash");
    sfxSource.PlayOneShot(dashSound);

    CameraShake.ShakeCamera(0.12f, 0.1f);

    float originalGravity = rb.gravityScale;

    rb.gravityScale = 0f;

    // Zatrzymuje pionowy ruch podczas dasha
    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

    float dashDirection = moveInput != 0 ? moveInput : (sr.flipX ? -1f : 1f);
    float elapsed = 0f;

    while (elapsed < dashDuration)
    {
        float step = dashForce * Time.fixedDeltaTime;
        Vector2 nextPos = rb.position + new Vector2(step * dashDirection, 0f);

        Collider2D hit = Physics2D.OverlapCircle(
            nextPos,
            0.2f,
            groundLayer
        );

        if (hit != null)
        {
            break;
        }

        rb.MovePosition(nextPos);

        elapsed += Time.fixedDeltaTime;

        yield return new WaitForFixedUpdate();
    }

    rb.gravityScale = originalGravity;

    // Najważniejsze — brak pionowego podskoku po dashu
    rb.linearVelocity = new Vector2(
        rb.linearVelocity.x,
        0f
    );

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
    if (isChargingAttack)
{
    CancelChargeAttack();
}
    anim.ResetTrigger("Attack1");
    anim.ResetTrigger("Attack2");
    anim.ResetTrigger("Attack3");

    if (isAttacking) return;

    isAttacking = true;
    attackFacingDirection = sr.flipX ? -1f : 1f;

    int randomAttack = Random.Range(1, 3);

    switch (randomAttack)
    {
        case 1:
            anim.SetTrigger("Attack1");
            sfxSource.PlayOneShot(attackSound1);
            break;

        case 2:
            anim.SetTrigger("Attack2");
            sfxSource.PlayOneShot(attackSound2);
            break;

        case 3:
            anim.SetTrigger("Attack3");
            sfxSource.PlayOneShot(attackSound1); // możesz zmienić jeśli chcesz 3 różne
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
        sfxSource.PlayOneShot(ghostStartSound);

        Vector3 spawnPosition = transform.position;
        activeGhost = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);

        PlayerGhost ghostScript = activeGhost.GetComponent<PlayerGhost>();

        if (ghostScript != null)
        {
            ghostScript.moveSpeed = moveSpeed * ghostMoveSpeedMultiplier;
            ghostScript.jumpForce = jumpForce * ghostJumpMultiplier;
            ghostScript.groundLayer = groundLayer;
            ghostScript.groundCheckRadius = groundCheckRadius;
        }

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
        sfxSource.PlayOneShot(ghostEndSound);

        yield return new WaitForSeconds(ghostCooldown);
        canUseGhost = true;
    }

    private IEnumerator SyncGhostWithPlayer(PlayerGhost ghost)
    {
        while (ghost != null)
        {
            ghost.SetMoveInput(moveInput);

            float direction = sr.flipX ? -1f : 1f;
            ghost.transform.localScale = new Vector3(direction, 1f, 1f);

            if (Input.GetKeyDown(KeyCode.E) && canDash)
            {
                float dashDir = moveInput != 0 ? moveInput : direction;
                ghost.StartDash(dashDir, dashForce * ghostDashMultiplier, dashDuration);
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
        sfxSource.PlayOneShot(rewindStartSound);

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

        if (ghost != null)
            Destroy(ghost);

        staticGhostActive = false;
        abilityInUse = false;
        anim.SetBool("SkillMode", false);
        sfxSource.PlayOneShot(rewindEndSound);

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

    private void PlayFootstep()
    {
        if (footstepSounds.Length == 0) return;

        int i = Random.Range(0, footstepSounds.Length);
        footstepSource.PlayOneShot(footstepSounds[i]);
    }

    private void StartChargeAttack()
{
    anim.ResetTrigger("ChargeRelease");
    anim.ResetTrigger("ChargeStart");

    isChargingAttack = true;
    currentChargeTime = 0f;

    anim.SetTrigger("ChargeStart");

    rb.linearVelocity = Vector2.zero;

    // START ŁADOWANIA SOUND
    if (chargeLoopSound != null)
    {
        chargeAudioSource.clip = chargeLoopSound;
        chargeAudioSource.Play();
    }
}

private void ChargeAttack()
{
    currentChargeTime += Time.deltaTime;

    if (currentChargeTime > maxChargeTime)
        currentChargeTime = maxChargeTime;

    // CAMERA SHAKE PODCZAS ŁADOWANIA
    float shakeStrength = Mathf.Lerp(0.01f, 0.05f, currentChargeTime / maxChargeTime);

    CameraShake.ShakeCamera(shakeStrength, 0.05f);
}

private IEnumerator ReleaseChargeAttack()
{
    isChargingAttack = false;

    // STOP ładowania sound
    if (chargeAudioSource.isPlaying)
    {
        chargeAudioSource.Stop();
    }

    // RELEASE SOUND
    if (chargeReleaseSound != null)
    {
        sfxSource.PlayOneShot(chargeReleaseSound);
    }

    anim.SetTrigger("ChargeRelease");

    isDashing = true;
    isInvulnerable = true;
    canUseChargedAttack = false;

    StartCoroutine(ChargedAttackCooldown());

    hitEnemiesDuringCharge.Clear();

    float chargePercent = currentChargeTime / maxChargeTime;

    float finalSpeed = Mathf.Lerp(
        chargedDashForce * 0.5f,
        chargedDashForce,
        chargePercent
    );

    float direction = sr.flipX ? -1f : 1f;

    float timer = 0f;

    while (timer < chargedDashDuration)
    {
        rb.linearVelocity = new Vector2(
            direction * finalSpeed,
            rb.linearVelocity.y
        );

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position,
            1.2f,
            enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            EnemyHealth hp = enemy.GetComponent<EnemyHealth>();

            if (hp != null && !hitEnemiesDuringCharge.Contains(hp))
            {
                hitEnemiesDuringCharge.Add(hp);

                int finalDamage = Mathf.RoundToInt(
                    Mathf.Lerp(
                        minChargeDamage,
                        maxChargeDamage,
                        chargePercent
                    )
                );

                hp.TakeDamage(finalDamage);

                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();

                if (enemyRb != null)
                {
                    Vector2 knockDir = new Vector2(direction, 0.2f).normalized;

                    enemyRb.AddForce(knockDir * 10f, ForceMode2D.Impulse);
                }
            }
        }

        timer += Time.deltaTime;
        yield return null;
    }

    isInvulnerable = false;
    isDashing = false;

    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
}

private void CancelChargeAttack()
{
    isChargingAttack = false;

    currentChargeTime = 0f;

    anim.SetBool("ChargingAttack", false);
    if (chargeAudioSource.isPlaying)
{
    chargeAudioSource.Stop();
}

    anim.ResetTrigger("ChargeStart");
    anim.ResetTrigger("ChargeRelease");
}

private IEnumerator ChargedAttackCooldown()
{
    yield return new WaitForSeconds(chargedAttackCooldown);

    canUseChargedAttack = true;
}

}