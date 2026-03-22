using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("Damage Control")]
public bool canTakeDamage = true; // uniwersalne blokowanie obrażeń

    [Header("Flash")]
    public int flashCount = 3;
    public Color flashColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("Drops")]
    public GameObject coinPrefab;
    public int minCoins = 1;
    public int maxCoins = 5;

    [Header("Animations")]
    public Animator animator;
    public string deathAnimationTrigger = "Death";

    [HideInInspector]
    public bool IsDead = false;

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
    if (IsDead || !canTakeDamage) return; // KLUCZOWA ZMIANA

    currentHealth -= amount;
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    StartCoroutine(FlashCoroutine());

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
        if (IsDead) return;

        IsDead = true;

        // Zatrzymanie fizyki
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0f;
            rb2D.gravityScale = 0f;
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (col2D != null)
            col2D.enabled = false;

        // Uruchom animację śmierci
        if (animator != null && !string.IsNullOrEmpty(deathAnimationTrigger))
        {
            animator.SetTrigger(deathAnimationTrigger);
        }

        

        // Wyłącz wszystkie skrypty oprócz tego
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this) script.enabled = false;
        }

        // Usuń dzieci od razu
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Nie używamy DestroyAfterAnimation, teraz czekamy na event z animacji
    }

    void DropCoins()
    {
        if (coinPrefab == null) return;

        int coinsToDrop = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < coinsToDrop; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.1f, 0.3f), 0);
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float forceX = Random.Range(-1f, 1f);
                float forceY = Random.Range(2f, 4f);
                rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
            }
        }
    }

    // Ta metoda powinna być wywołana z eventu animacji "Death" na końcu animacji
    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    public void DropCoinsAnimation()
    {
        // Drop coins natychmiast
        DropCoins();
    }
}