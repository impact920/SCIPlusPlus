using UnityEngine;

public class EnemyHiding : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    private bool movingRight = true;

    [Header("Ground Detection")]
    public Transform groundDetection;
    public float detectionDistance = 2f;
    public LayerMask groundLayer;

    [Header("Wall Detection")]
    public Transform wallDetection;
    public float wallDetectionDistance = 0.5f;

    [Header("Awake Settings")]
    public Transform player;
    public float wakeUpDistance = 5f;
    private bool isAwake = false;

    [Header("Explosion Settings")]
    public float timeToExplode = 5f;
    public GameObject dropPrefab;
    public GameObject explosionEffect;

    private float timer;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = timeToExplode;
    }

    void Update()
    {
        CheckWakeUp();

        if (!isAwake)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        HandleMovement();
        HandleExplosion();
    }

    // -------------------------
    // WYBUDZANIE + ANIMACJA
    // -------------------------
    void CheckWakeUp()
    {
        if (player == null || isAwake) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= wakeUpDistance)
        {
            isAwake = true;

            // trigger animacji przebudzenia
            animator.SetTrigger("WakeUp");
        }
    }

    // -------------------------
    // RUCH + ANIMACJA
    // -------------------------
    void HandleMovement()
    {
        rb.velocity = new Vector2(moveSpeed * (movingRight ? 1 : -1), rb.velocity.y);

        animator.SetBool("isWalking", true);

        isGrounded = Physics2D.Raycast(groundDetection.position, Vector2.down, detectionDistance, groundLayer);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, detectionDistance, groundLayer);
        if (groundInfo.collider == false)
        {
            Flip();
        }

        RaycastHit2D wallInfo = Physics2D.Raycast(
            wallDetection.position,
            movingRight ? Vector2.right : Vector2.left,
            wallDetectionDistance,
            groundLayer
        );

        if (wallInfo.collider == true)
        {
            Flip();
        }
    }

    // -------------------------
    // WYBUCH + ANIMACJA
    // -------------------------
    void HandleExplosion()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Explode();
        }
    }

    void Explode()
{
    animator.SetTrigger("Explode");

    rb.velocity = Vector2.zero;

    // wyłączamy collider od razu
    GetComponent<Collider2D>().enabled = false;
}

    public void SpawnAfterExplosion()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        if (dropPrefab != null)
        {
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
        }

    }

    void Flip()
    {
        movingRight = !movingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    
}