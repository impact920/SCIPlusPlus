using UnityEngine;

public class EnemyChasing : MonoBehaviour
{
    public Transform player;       
    public float chaseRange = 5f;  
    public float attackRange = 1f; 
    public float speed = 2f;       
    public float attackCooldown = 1f; 

    private Rigidbody2D rb;
    private Animator anim;
    private float attackTimer = 0f;
    private EnemyHealth enemyHealth;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        // Jeśli przeciwnik martwy — całkowity stop
        if (enemyHealth != null && enemyHealth.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat("Speed", 0f);
            return;
        }

        if (player == null) return;

        float distanceX = player.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);

        // Odliczanie cooldownu ataku
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (absDistanceX <= attackRange && attackTimer <= 0f)
        {
            // Atak
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetTrigger("Attack");
            anim.SetFloat("Speed", 0f);
            attackTimer = attackCooldown;
        }
        else if (absDistanceX <= chaseRange)
        {
            // Pościg
            float move = Mathf.Sign(distanceX) * speed;
            rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);

            // Obrót w stronę gracza
            Vector3 scale = transform.localScale;
            scale.x = move > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;

            anim.SetFloat("Speed", Mathf.Abs(move));
        }
        else
        {
            // Idle
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("Speed", 0f);
        }
    }
}
