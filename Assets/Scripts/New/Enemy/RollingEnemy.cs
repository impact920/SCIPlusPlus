using UnityEngine;

public class SnailEnemy : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float rollSpeed = 4f;
    public float patrolDistance = 3f;

    [Header("Smoothing")]
    public float acceleration = 8f;
    public float deceleration = 10f;
    public float directionSmooth = 0.05f;

    [Header("Detection")]
    public float detectRange = 5f;

    [Header("Attack")]
    public float prepareRollTime = 0.5f; // 🔥 czas na animację CLOSE
    public float rollTime = 1.5f;
    public float restTime = 2f;

    private Rigidbody2D rb;
    private Animator anim;
    private EnemyHealth enemyHealth;

    private Vector2 startPos;
    private bool movingRight = true;

    private Vector2 velocity;
    private Vector2 direction;

    private float stateTimer;

    private enum State
    {
        Patrol,
        PrepareRoll, // 🔥 NOWY STAN (zamykanie)
        Roll,
        Rest
    }

    private State currentState = State.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();

        startPos = transform.position;
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetTrigger("Death");
            return;
        }

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();

                if (distance <= detectRange)
                {
                    StartPrepareRoll();
                }
                break;

            case State.PrepareRoll:
                PrepareRoll();
                break;

            case State.Roll:
                Roll();
                break;

            case State.Rest:
                Rest();
                break;
        }
    }

    // ================= PATROL =================
    void Patrol()
    {
        anim.SetBool("isWalking", true);
        anim.SetBool("isRolling", false);
        anim.SetBool("isResting", false);

        float targetDir = movingRight ? 1f : -1f;

        direction = Vector2.Lerp(direction, new Vector2(targetDir, 0), directionSmooth).normalized;

        velocity.x = Mathf.MoveTowards(velocity.x, direction.x * patrolSpeed, acceleration * Time.deltaTime);

        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);

        if (movingRight && transform.position.x >= startPos.x + patrolDistance)
            Flip(false);
        else if (!movingRight && transform.position.x <= startPos.x - patrolDistance)
            Flip(true);
    }

    // ================= PREPARE ROLL (CLOSE ANIMATION) =================
    void StartPrepareRoll()
    {
        currentState = State.PrepareRoll;
        stateTimer = prepareRollTime;

        velocity = Vector2.zero;

        anim.SetBool("isWalking", false);
        anim.SetTrigger("Close"); // 🔥 animacja zamykania
    }

    void PrepareRoll()
    {
        stateTimer -= Time.deltaTime;

        // lekkie hamowanie (żeby nie było nagłego stopu)
        velocity = Vector2.MoveTowards(velocity, Vector2.zero, deceleration * Time.deltaTime);
        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);

        if (stateTimer <= 0)
        {
            StartRoll();
        }
    }

    // ================= ROLL =================
    void StartRoll()
    {
        enemyHealth.canTakeDamage = false;
        currentState = State.Roll;
        stateTimer = rollTime;

        anim.SetBool("isRolling", true);

        direction = (player.position - transform.position).normalized;
    }

    void Roll()
    {
        stateTimer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectRange)
        {
            Vector2 targetDir = (player.position - transform.position).normalized;

            direction = Vector2.Lerp(direction, targetDir, directionSmooth).normalized;
        }

        velocity = Vector2.MoveTowards(velocity, direction * rollSpeed, acceleration * Time.deltaTime);

        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);

        if (velocity.x != 0)
            Flip(velocity.x > 0);

        if (stateTimer <= 0)
        {
            StartRest();
        }
    }

    // ================= REST =================
    void StartRest()
    {
        currentState = State.Rest;
        stateTimer = restTime;

        anim.SetBool("isRolling", false);
        anim.SetBool("isResting", true);
        anim.SetTrigger("Open"); // wychodzi ze skorupy
    }

    void Rest()
    {
        stateTimer -= Time.deltaTime;

        // 🔥 tu można zadawać obrażenia ślimakowi
        if (enemyHealth != null)
            enemyHealth.canTakeDamage = true;

        velocity = Vector2.MoveTowards(velocity, Vector2.zero, deceleration * Time.deltaTime);
        rb.linearVelocity = new Vector2(velocity.x, rb.linearVelocity.y);

        if (stateTimer <= 0)
        {
            if (enemyHealth != null)
                enemyHealth.canTakeDamage = false;

            anim.SetBool("isResting", false);
            anim.SetTrigger("Close");

            currentState = State.Patrol;
        }
    }

    // ================= FLIP =================
    void Flip(bool faceRight)
    {
        movingRight = faceRight;

        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}