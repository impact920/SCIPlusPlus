using UnityEngine;

public class EnemyChasing : MonoBehaviour
{
    public Transform player;       
    public float chaseRange = 5f;  
    public float attackRange = 1f; 
    public float moveSpeed = 2f;   // <-- nowa nazwa dla czytelności
    public float attackCooldown = 1f; 
    public float directionDeadZone = 0.2f; // <-- zapobiega flipowaniu

    private Rigidbody2D rb;
    private Animator anim;
    private float attackTimer = 0f;
    private EnemyHealth enemyHealth;

    private int facingDirection = 1; // 1 = prawo, -1 = lewo

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
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

        float distanceX = player.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        // 🔥 Aktualizuj kierunek tylko jeśli różnica jest większa niż dead zone
        if (Mathf.Abs(distanceX) > directionDeadZone)
        {
            facingDirection = distanceX > 0 ? 1 : -1;
        }

        if (absDistanceX <= attackRange && attackTimer <= 0f)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetTrigger("Attack");
            anim.SetFloat("Speed", 0f);
            attackTimer = attackCooldown;
        }
        else if (absDistanceX <= chaseRange)
        {
            float move = facingDirection * moveSpeed;
            rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);

            // Obrót
            Vector3 scale = transform.localScale;
            scale.x = facingDirection > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;

            anim.SetFloat("Speed", Mathf.Abs(move));
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("Speed", 0f);
        }
    }
}