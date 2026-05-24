using UnityEngine;

public class EnemyTwoPhase : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Ground Phase")]
    public float groundChaseRange = 5f;
    public float groundAttackRange = 1f;
    public float groundMoveSpeed = 2f;

    [Header("Phase Change")]
[Range(0f, 1f)]
public float flyingPhaseHealthPercent = 0.5f;

    [Header("Flying Phase")]
    public float flyingSpeed = 3f;
    public float flyingAttackRange = 1.5f;
    public float flyingChaseRange = 7f;

    [Header("Flying Bounds")]
    public float minY = -2f;
    public float maxY = 2f;
    [Header("Flying Dash")]
public float dashSpeed = 10f;
public float dashDuration = 0.4f;
public float dashCooldown = 3f;
public float dashChargeTime = 0.25f;
public float dashAcceleration = 25f;
public float dashSlowdown = 15f;

    [Header("Combat")]
    public float attackCooldown = 1f;
    public float directionDeadZone = 0.2f;
    [Header("Random Dash Stats")]
public bool randomizeDashStats = true;
public Vector2 dashSpeedRange = new Vector2(8f, 14f);

public Vector2 dashCooldownRange = new Vector2(2f, 5f);

public Vector2 dashDurationRange = new Vector2(0.3f, 0.7f);

public Vector2 dashChargeTimeRange = new Vector2(0.1f, 0.4f);

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private EnemyHealth enemyHealth;

    private float attackTimer = 0f;

    private int facingDirection = 1;

    // flying
    private Vector2 flyingDirection;
    private bool isPreparingDash = false;

private float dashChargeTimer = 0f;

private float currentDashSpeed = 0f;

    private bool isFlyingPhase = false;
    private bool isDashing = false;

private float dashTimer = 0f;
private float dashCooldownTimer = 0f;

private Vector2 dashDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();

        float dirX = Random.value > 0.5f ? 1f : -1f;
        float dirY = Random.value > 0.5f ? 1f : -1f;

        flyingDirection = new Vector2(dirX, dirY).normalized;

        if (randomizeDashStats)
{
    dashSpeed = Random.Range(
        dashSpeedRange.x,
        dashSpeedRange.y
    );

    dashCooldown = Random.Range(
        dashCooldownRange.x,
        dashCooldownRange.y
    );

    dashDuration = Random.Range(
        dashDurationRange.x,
        dashDurationRange.y
    );

    dashChargeTime = Random.Range(
        dashChargeTimeRange.x,
        dashChargeTimeRange.y
    );
}
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat("Speed", 0f);
            return;
        }

        if (player == null) return;

        // przejście do 2 fazy
        CheckPhase();

        // cooldown
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (isFlyingPhase)
        {
            FlyingBehaviour();
        }
        else
        {
            GroundBehaviour();
        }
    }

    void CheckPhase()
{
    if (enemyHealth == null) return;

    float hpPercent =
        (float)enemyHealth.currentHealth / enemyHealth.maxHealth;

    if (!isFlyingPhase && hpPercent <= flyingPhaseHealthPercent)
    {
        StartFlyingPhase();
    }
}

    void StartFlyingPhase()
{
    isFlyingPhase = true;

    rb.gravityScale = 0f;

    anim.SetTrigger("FlyStart");

    dashCooldownTimer = dashCooldown;

    rb.linearVelocity = Vector2.zero;

    if (sr != null)
    {
        sr.flipX = player.position.x > transform.position.x;
    }

    Debug.Log("Enemy entered flying phase!");
}

    // =========================
    // GROUND PHASE
    // =========================
    void GroundBehaviour()
    {
        float distanceX = player.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);

        // update kierunku
        if (Mathf.Abs(distanceX) > directionDeadZone)
        {
            facingDirection = distanceX > 0 ? 1 : -1;
        }

        // ATAK
        if (absDistanceX <= groundAttackRange && attackTimer <= 0f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            anim.SetTrigger("Attack");
            anim.SetFloat("Speed", 0f);

            attackTimer = attackCooldown;
        }
        // CHODZENIE
        else if (absDistanceX <= groundChaseRange)
        {
            float move = facingDirection * groundMoveSpeed;

            rb.linearVelocity =
                new Vector2(move, rb.linearVelocity.y);

            if (sr != null)
{
    sr.flipX = facingDirection > 0;
}

            anim.SetFloat("Speed", Mathf.Abs(move));
        }
        else
        {
            rb.linearVelocity =
                new Vector2(0, rb.linearVelocity.y);

            anim.SetFloat("Speed", 0f);
        }
    }

    // =========================
    // FLYING PHASE
    // =========================
    void FlyingBehaviour()
{
    float distance =
        Vector2.Distance(transform.position, player.position);

    // cooldown
    if (dashCooldownTimer > 0)
        dashCooldownTimer -= Time.deltaTime;

    // =========================
    // PRZYGOTOWANIE DASHA
    // =========================
    if (isPreparingDash)
    {
        rb.linearVelocity = Vector2.zero;

        dashChargeTimer -= Time.deltaTime;

        // patrz na gracza
        Vector2 lookDir =
            (player.position - transform.position).normalized;

        if (sr != null)
        {
            sr.flipX = lookDir.x > 0;
        }

        if (dashChargeTimer <= 0f)
        {
            isPreparingDash = false;
            isDashing = true;

            currentDashSpeed = 0f;

            dashTimer = dashDuration;
        }

        return;
    }

    // =========================
    // DASH
    // =========================
    if (isDashing)
    {
        dashTimer -= Time.deltaTime;

        // płynne przyspieszanie
        currentDashSpeed = Mathf.MoveTowards(
            currentDashSpeed,
            dashSpeed,
            dashAcceleration * Time.deltaTime
        );

        rb.linearVelocity =
            dashDirection * currentDashSpeed;

        // flip
        if (sr != null)
        {
            sr.flipX = dashDirection.x > 0;
        }

        // atak
        if (distance <= flyingAttackRange && attackTimer <= 0f)
        {
            anim.SetTrigger("Attack");
            attackTimer = attackCooldown;
        }

        CheckYBounds();

        // koniec dash
        if (dashTimer <= 0f)
        {
            isDashing = false;
        }

        return;
    }

    // =========================
    // NORMALNE LATANIE
    // =========================
    if (distance <= flyingChaseRange)
    {
        Vector2 targetDir =
            (player.position - transform.position).normalized;

        flyingDirection =
            Vector2.Lerp(flyingDirection, targetDir, 0.03f).normalized;
    }

    // płynny ruch
    rb.linearVelocity = Vector2.Lerp(
        rb.linearVelocity,
        flyingDirection * flyingSpeed,
        dashSlowdown * Time.deltaTime
    );

    // flip
    if (sr != null)
    {
        sr.flipX = flyingDirection.x > 0;
    }

    // =========================
    // START DASHA
    // =========================
    if (distance <= flyingChaseRange &&
        dashCooldownTimer <= 0f)
    {
        anim.SetTrigger("FlyDash");

        dashDirection =
            (player.position - transform.position).normalized;

        dashCooldownTimer = dashCooldown;

        isPreparingDash = true;
        dashChargeTimer = dashChargeTime;
    }

    CheckYBounds();
}

    void CheckYBounds()
    {
        if (transform.position.y >= maxY && flyingDirection.y > 0)
        {
            flyingDirection.y *= -1;
        }
        else if (transform.position.y <= minY && flyingDirection.y < 0)
        {
            flyingDirection.y *= -1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFlyingPhase) return;

        if (enemyHealth != null && enemyHealth.IsDead)
            return;

        Vector2 normal = collision.contacts[0].normal;

        flyingDirection =
            Vector2.Reflect(flyingDirection, normal).normalized;
    }
}