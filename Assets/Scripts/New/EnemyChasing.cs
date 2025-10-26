using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public Transform player;       
    public float chaseRange = 5f;  
    public float attackRange = 1f; 
    public float speed = 2f;       
    public float attackCooldown = 1f; 

    private Rigidbody2D rb;
    private Animator anim;
    private float attackTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
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
            anim.SetTrigger("Attack"); // Użycie Triggera zamiast Bool
            anim.SetFloat("Speed", 0f);
            attackTimer = attackCooldown;
        }
        else if (absDistanceX <= chaseRange)
        {
            // Pościg (Walk)
            float move = Mathf.Sign(distanceX) * speed;
            rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);

            // Odwrócenie przeciwnika w stronę gracza
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
