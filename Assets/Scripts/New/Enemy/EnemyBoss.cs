using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public Transform player;

    [Header("Combat")]
    public float chaseRange = 6f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float directionDeadZone = 0.2f;

    [Header("Summon Trigger Range")]
    public float summonRange = 4f; // 🔥 NOWE: kiedy boss przywołuje

    [Header("Movement Scaling")]
    public float minMoveSpeed = 2f;
    public float maxMoveSpeed = 5f;

    [Header("Enemies")]
    public GameObject[] easyEnemies;
    public GameObject[] mediumEnemies;
    public GameObject[] hardEnemies;

    [Header("Spawn Points (STRICT by difficulty)")]
    public Transform[] easySpawnPoints;
    public Transform[] mediumSpawnPoints;
    public Transform[] hardSpawnPoints;

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
        if (player == null || enemyHealth == null || enemyHealth.IsDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (!isAwake)
        {
            if (distanceToPlayer <= chaseRange)
                isAwake = true;
            else
                return;
        }

        float healthPercent = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;

        float currentMoveSpeed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, 1 - healthPercent);
        float currentSummonCooldown = Mathf.Lerp(maxSummonCooldown, minSummonCooldown, 1 - healthPercent);

        float distanceX = player.position.x - transform.position.x;
        float absDistanceX = Mathf.Abs(distanceX);

        if (attackTimer > 0) attackTimer -= Time.deltaTime;
        if (summonTimer > 0) summonTimer -= Time.deltaTime;

        if (Mathf.Abs(distanceX) > directionDeadZone)
            facingDirection = distanceX > 0 ? 1 : -1;

        // =========================
        // SUMMON (NOW RANGE-BASED)
        // =========================
        if (summonTimer <= 0f && distanceToPlayer <= summonRange)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetTrigger("Summon");
            summonTimer = currentSummonCooldown;
            return;
        }

        // ATTACK
        if (absDistanceX <= attackRange && attackTimer <= 0f)
        {
            rb.linearVelocity = Vector2.zero;

            anim.SetTrigger(Random.Range(0, 2) == 0 ? "Attack1" : "Attack2");

            attackTimer = attackCooldown;
        }
        else if (absDistanceX <= chaseRange)
        {
            float move = facingDirection * currentMoveSpeed;
            rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);

            Vector3 scale = transform.localScale;
            scale.x = facingDirection > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // =========================
    // SPAWN SYSTEM
    // =========================
    public void SpawnEnemies()
    {
        if (enemyHealth == null) return;

        int spawnCount = Random.Range(2, 6);
        float healthPercent = (float)enemyHealth.currentHealth / enemyHealth.maxHealth;

        for (int i = 0; i < spawnCount; i++)
        {
            GameObject prefab = GetEnemyByDifficulty(healthPercent);
            Transform spawnPoint = GetSpawnPointForEnemy(prefab);

            if (prefab == null || spawnPoint == null)
                continue;

            GameObject enemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

            EnemyChasing aic = enemy.GetComponent<EnemyChasing>();
            if (aic != null) aic.player = player;

            EnemyFlying aif = enemy.GetComponent<EnemyFlying>();
            if (aif != null) aif.player = player;
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

    Transform GetSpawnPointForEnemy(GameObject enemyPrefab)
    {
        if (IsInArray(enemyPrefab, easyEnemies))
            return GetRandomPoint(easySpawnPoints);

        if (IsInArray(enemyPrefab, mediumEnemies))
            return GetRandomPoint(mediumSpawnPoints);

        if (IsInArray(enemyPrefab, hardEnemies))
            return GetRandomPoint(hardSpawnPoints);

        return null;
    }

    bool IsInArray(GameObject obj, GameObject[] array)
    {
        foreach (var item in array)
        {
            if (item == obj)
                return true;
        }
        return false;
    }

    Transform GetRandomPoint(Transform[] points)
    {
        if (points == null || points.Length == 0)
            return null;

        return points[Random.Range(0, points.Length)];
    }

    // =========================
    // GIZMOS (EDITOR VISUAL)
    // =========================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, summonRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}