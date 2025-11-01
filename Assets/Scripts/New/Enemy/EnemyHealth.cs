using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int damage = 10;
    public int maxHealth = 50;
    public int currentHealth;

    public int flashCount = 3;
    public Color flashColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Animacje")]
    public Animator animator;
    public string deathAnimationTrigger = "Death";
    public float deathAnimationDuration = 1.0f;

    [HideInInspector] public bool IsDead = false;

    private Rigidbody2D rb2D;
    private Collider2D col2D;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2D = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(FlashCoroutine());
    }

    public IEnumerator FlashCoroutine()
    {
        if (spriteRenderer == null) yield break;

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;

        // Zatrzymanie fizyki i kolizji
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
            rb2D.gravityScale = 0f;
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (col2D != null)
            col2D.enabled = false;

        // Animacja śmierci
        if (animator != null && !string.IsNullOrEmpty(deathAnimationTrigger))
        {
            animator.SetTrigger(deathAnimationTrigger);
        }

        // Zniszczenie po czasie trwania animacji
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }
}
