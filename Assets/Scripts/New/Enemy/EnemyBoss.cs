using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public Transform player;

    [Header("Combat")]
    public float chaseRange = 6f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float directionDeadZone = 0.2f;

    [Header("Movement Scaling")]
    public float minMoveSpeed = 2f;
    public float maxMoveSpeed = 5f;

    [Header("Summon Enemies")]
    public GameObject[] easyEnemies;
    public GameObject[] mediumEnemies;
    public GameObject[] hardEnemies;

    [Header("Spawn Settings")]
    public float spawnRadius = 6f;
    public float groundCheckDistance = 15f;
    public float maxGroundHeightDifference = 2f;

    public LayerMask groundLayer;
    public LayerMask obstacleLayer;

    [Header("Summon Scaling")]
    public float maxSummonCooldown = 5f;
    public float minSummonCooldown = 2f;

    private float summonTimer = 0f;
    private float attackTimer = 0f;

    private Rigidbody2D rb;
    private Animator anim;
    private EnemyHealth enemyHealth;

    private int facingDirection = 1;

    private bool isAwake = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

if (!isAwake)
{
    if (distanceToPlayer <= chaseRange)
    {
        isAwake = true;
        anim.SetTrigger("WakeUp");
    }
    else
    {
        return;
    }
}
        
        if (enemyHealth == null || enemyHealth.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (player == null) return;

        float healthPercent = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;

        float currentMoveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, 1 - healthPercent);
        float currentSummonCooldown = Mathf.Lerp(maxSummonCooldown, minSummonCooldown, 1 - healthPercent);

        float distanceX = player.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);

        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (summonTimer > 0)
            summonTimer -= Time.deltaTime;

        // kierunek patrzenia
        if (Mathf.Abs(distanceX) > directionDeadZone)
        {
            facingDirection = distanceX > 0 ? 1 : -1;
        }

        // SUMMON
        if (summonTimer <= 0f && absDistanceX <= chaseRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetTrigger("Summon");
            summonTimer = currentSummonCooldown;
            return;
        }

        // ATAK
        if (absDistanceX <= attackRange && attackTimer <= 0f)
        {
            rb.linearVelocity = Vector2.zero;

            if (Random.Range(0, 2) == 0)
                anim.SetTrigger("Attack1");
            else
                anim.SetTrigger("Attack2");

            attackTimer = attackCooldown;
        }
        else if (absDistanceX <= chaseRange)
        {
            float move = facingDirection * currentMoveSpeed;
            rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);

            // OBRÓT
            Vector3 scale = transform.localScale;
            scale.x = facingDirection > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;

        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // ANIMATION EVENT (SUMMON)
    public void SpawnEnemies()
{
    if (enemyHealth == null) return;

    int spawnCount = Random.Range(2, 6);

    float healthPercent = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;

    for (int i = 0; i < spawnCount; i++)
    {
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;

        Vector2 origin = (Vector2)transform.position + randomOffset + Vector2.up * 5f;

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 20f, groundLayer);

        Debug.DrawRay(origin, Vector2.down * 20f, Color.red, 1f);

        if (hit.collider == null)
            continue;

        // anti-stuck (przeszkody)
        Collider2D obstacle = Physics2D.OverlapCircle(hit.point + Vector2.up * 0.5f, 0.4f, obstacleLayer);

        if (obstacle != null)
            continue;

        GameObject prefab = GetEnemyByDifficulty(healthPercent);

        Vector3 spawnPos = hit.point;
        spawnPos.y += 0.2f;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        //  KLUCZOWA NAPRAWA: przypisanie gracza
        EnemyChasing aic = enemy.GetComponent<EnemyChasing>();
        if (aic != null)
        {
            aic.player = player;
        }
        EnemyFlying aif = enemy.GetComponent<EnemyFlying>();
        if (aif != null)
        {
            aif.player = player;
        }
    }
}

    GameObject GetEnemyByDifficulty(float hpPercent)
    {
        float rand = Random.value;

        if (hpPercent > 0.6f)
        {
            return easyEnemies[Random.Range(0, easyEnemies.Length)];
        }
        else if (hpPercent > 0.3f)
        {
            return rand < 0.6f
                ? easyEnemies[Random.Range(0, easyEnemies.Length)]
                : mediumEnemies[Random.Range(0, mediumEnemies.Length)];
        }
        else
        {
            return rand < 0.4f
                ? mediumEnemies[Random.Range(0, mediumEnemies.Length)]
                : hardEnemies[Random.Range(0, hardEnemies.Length)];
        }
    }
}