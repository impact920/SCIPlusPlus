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
    public Animator animator; // przypisz Animator w Inspectorze
    public string deathAnimationTrigger = "Death"; // trigger animacji śmierci
    public float deathAnimationDuration = 1.0f; // czas trwania animacji śmierci w sekundach

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
            Die();
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
        if (isDead) return;
        isDead = true;

        // Wywołanie animacji śmierci
        if (animator != null && !string.IsNullOrEmpty(deathAnimationTrigger))
        {
            animator.SetTrigger(deathAnimationTrigger);
            // Uruchom Coroutine, która zniszczy obiekt po zakończeniu animacji
            StartCoroutine(DestroyAfterAnimation());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // Czekamy długość animacji
        yield return new WaitForSeconds(deathAnimationDuration);
        Destroy(gameObject);
    }
}
