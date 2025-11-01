using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Ustawienia zdrowia")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Ochrona po obrażeniach")]
    public float invincibilityDuration = 1.5f;
    public int flashCount = 5;
    public Color flashColor = new Color(1, 0, 0, 0.5f);

    [Header("Efekty i dźwięki")]
    public GameObject deathEffect;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public int attackDamage = 20;
    public float knockbackForce = 5f;
    public float Reach = 5f;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isInvincible = false;
    private bool isDead = false;

    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (isDead) return;

        HandleFlip();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    // ✅ Obracanie postaci na podstawie prędkości (działa z dashowaniem)
    void HandleFlip()
    {
        if (rb.linearVelocity.x > 0.1f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (rb.linearVelocity.x < -0.1f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Attack()
{
    float direction = Mathf.Sign(transform.localScale.x);
    Vector2 capsuleCenter = (Vector2)transform.position + new Vector2(direction * (Reach / 2), 0f);
    Vector2 capsuleSize = new Vector2(Reach, 2f); // Możesz ustawić wysokość kapsuły np. = wysokość postaci

    Collider2D[] hitEnemies = Physics2D.OverlapCapsuleAll(
        capsuleCenter,
        capsuleSize,
        CapsuleDirection2D.Horizontal,
        0f
    );

    foreach (var enemyCollider in hitEnemies)
    {
        EnemyHealth enemy = enemyCollider.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            Vector2 toEnemy = enemy.transform.position - transform.position;
            if (Mathf.Sign(toEnemy.x) == direction)
            {
                enemy.TakeDamage(attackDamage);
                enemy.StartCoroutine(enemy.FlashCoroutine());

                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 knockbackDir = toEnemy.normalized;
                    enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }
}


    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (audioSource && hitSound)
            audioSource.PlayOneShot(hitSound);

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibilityCoroutine());
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (audioSource && deathSound)
            audioSource.PlayOneShot(deathSound);

        if (deathEffect)
            Instantiate(deathEffect, transform.position, Quaternion.identity);
    }

    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        for (int i = 0; i < flashCount; i++)
        {
            if (spriteRenderer)
                spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(invincibilityDuration / (flashCount * 2));
            if (spriteRenderer)
                spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(invincibilityDuration / (flashCount * 2));
        }

        isInvincible = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            TakeDamage(enemyHealth.damage);
        }
    }

    void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;

    float direction = Mathf.Sign(transform.localScale.x);
    Vector2 capsuleCenter = (Vector2)transform.position + new Vector2(direction * (Reach / 2), 0f);
    Vector2 capsuleSize = new Vector2(Reach, 2f);

    // Rysuj prostokąt, żeby zobaczyć obszar kapsuły
    Gizmos.DrawWireCube(capsuleCenter, capsuleSize);
}

}
