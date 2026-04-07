using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Invincibility")]
    public float invincibilityDuration = 1.5f;
    public int flashCount = 5;
    public Color flashColor = new Color(1, 0, 0, 0.5f);

    [Header("Sounds and Effects")]
    public GameObject deathEffect;
    public AudioClip hitSound;
    public AudioClip deathSound;

    [Header("Attack")]
    public int attackDamage = 20;
    public float knockbackForce = 5f;
    public float Reach = 5f;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isInvincible = false;
    private bool isDead = false;
    private Rigidbody2D rb;

    // Pola dla systemu checkpointów i Death Menu
    private DeathManager deathManager;
    private PlayerRespawn playerRespawn;

    private PlayerMovement playerMovement;

    [Header("Currency Loss On Death")]
[Range(0f, 100f)] public float minCurrencyLossPercent = 10f;
[Range(0f, 100f)] public float maxCurrencyLossPercent = 20f;


    void Start()
    {

        if (PlayerPrefs.HasKey("PlayerHealth"))
        {   
        currentHealth = PlayerPrefs.GetInt("PlayerHealth");
        }
        else
        {
        currentHealth = maxHealth;
        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        }

        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        deathManager = FindObjectOfType<DeathManager>();
        playerRespawn = GetComponent<PlayerRespawn>();

        playerMovement = GetComponent<PlayerMovement>(); // dodaj to

    }

    void Update()
    {
        if (isDead) return;
    }

    

    public void DealDamage()
{
    CameraShake.Instance?.ShakeFromDamage(attackDamage);

    float direction = spriteRenderer.flipX ? -1f : 1f;

    Vector2 capsuleCenter = (Vector2)transform.position + new Vector2(direction * (Reach / 2), 0f);
    Vector2 capsuleSize = new Vector2(Reach, 2f);

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
    // jeśli gracz jest martwy, ma invincibility lub dashuje → ignoruj
    if (isDead || isInvincible || (playerMovement != null && playerMovement.IsDashing()))
        return;

    currentHealth -= damage;
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    if (audioSource && hitSound)
        audioSource.PlayOneShot(hitSound);

    if (CameraShake.Instance != null)
        CameraShake.Instance.ShakeFromDamage(damage);

    if (currentHealth <= 0)
        Die();
    else
{
    StartCoroutine(InvincibilityCoroutine());
}

PlayerPrefs.SetInt("PlayerHealth", currentHealth);
}


    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
    }

    void Die()
{
    if (isDead) return;
    isDead = true;

    LoseCurrencyOnDeath(); // DODAJ TUTAJ

    if (deathEffect)
        Instantiate(deathEffect, transform.position, Quaternion.identity);

    if (audioSource && deathSound)
        audioSource.PlayOneShot(deathSound);

    StartCoroutine(ShowDeathMenuAfterDelay(1f));
}

    IEnumerator ShowDeathMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (deathManager != null)
            deathManager.ShowDeathMenu();
        else
            SceneManager.LoadScene("Main_Menu"); // Awaryjnie
            PlayerPrefs.SetInt("PlayerHealth", maxHealth);
    }

    public void Revive()
    {
        Debug.Log("REVIVE: wywołano odrodzenie gracza");

        currentHealth = maxHealth;
        isDead = false;

        if (playerRespawn != null)
        {
            Debug.Log("REVIVE: playerRespawn znaleziony, odradzam gracza");
            playerRespawn.Respawn();
        }
        else
        {
            Debug.LogError("REVIVE: Brak referencji do PlayerRespawn!");
        }
        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
    }

    private IEnumerator InvincibilityCoroutine()
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


    void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;

    float direction = transform.localScale.x >= 0 ? 1f : -1f;

    Vector2 capsuleCenter = (Vector2)transform.position + new Vector2(direction * (Reach / 2), 0f);
    Vector2 capsuleSize = new Vector2(Reach, 2f);

    Gizmos.DrawWireCube(capsuleCenter, capsuleSize);
}

void LoseCurrencyOnDeath()
{
    if (CurrencyManager.instance == null) return;

    float percent = Random.Range(minCurrencyLossPercent, maxCurrencyLossPercent);
    int currentMoney = CurrencyManager.instance.currency;

    int amountToLose = Mathf.RoundToInt(currentMoney * (percent / 100f));

    CurrencyManager.instance.AddCurrency(-amountToLose);

    Debug.Log($"Stracono {amountToLose} waluty ({percent}%)");
}

}
