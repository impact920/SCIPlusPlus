using UnityEngine;

public class EnemyFlying : MonoBehaviour
{
    public Transform player;

    public float speed = 3f;
    public float minY = -2f;
    public float maxY = 2f;

    public float chaseRange = 5f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    private Vector2 direction;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private float attackTimer = 0f;
    private bool isDead = false;
    private EnemyHealth enemyHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();

        float dirX = Random.value > 0.5f ? 1f : -1f;
        float dirY = Random.value > 0.5f ? 1f : -1f;

        direction = new Vector2(dirX, dirY).normalized;

        rb.gravityScale = 0f;
        anim.SetBool("isFlying", true);
    }

    void FixedUpdate()
    {
        // Śmierć
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // cooldown
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        // ATAK
        if (distance <= attackRange && attackTimer <= 0f)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetTrigger("Attack");
            attackTimer = attackCooldown;
            return;
        }

        // CHASE (lekko w stronę gracza)
        if (distance <= chaseRange)
        {
            Vector2 targetDir = (player.position - transform.position).normalized;

            // mieszamy ruch ukośny + lekkie podążanie
            direction = Vector2.Lerp(direction, targetDir, 0.05f).normalized;
        }

        // RUCH
        rb.linearVelocity = direction * speed;

        // flip sprite
        sr.flipX = direction.x < 0;

        CheckYBounds();
    }

    void CheckYBounds()
    {
        if (transform.position.y >= maxY && direction.y > 0)
        {
            direction.y *= -1;
        }
        else if (transform.position.y <= minY && direction.y < 0)
        {
            direction.y *= -1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enemyHealth != null && enemyHealth.IsDead)
            return;

        Vector2 normal = collision.contacts[0].normal;
        direction = Vector2.Reflect(direction, normal).normalized;
    }
}