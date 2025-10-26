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
    public int attackDamage = 20; // obrażenia zadawane przeciwnikowi
    public float knockbackForce = 5f; // siła odepchnięcia wroga

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isInvincible = false;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (isDead) return;

        // Atak lewym przyciskiem myszy
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
        // Sprawdzenie kolizji z wrogami w zasięgu 1 jednostki
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var enemyCollider in hitEnemies)
        {
            EnemyHealth enemy = enemyCollider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                enemy.StartCoroutine(enemy.FlashCoroutine());

                // Odepchnięcie wroga
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
                    enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
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

        Debug.Log($"Gracz otrzymał {damage} obrażeń. HP: {currentHealth}");

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

        Debug.Log("Gracz zginął!");
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

    // Wizualizacja zasięgu ataku w edytorze
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
