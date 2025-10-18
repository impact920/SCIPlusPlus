using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Tryb pocisku")]
    public bool damageMode = true; // Tryb zadawania obrażeń
    public bool pushMode = true; // Tryb odpychania
    public float pushForce = 5f; // Siła odpychania

    [Header("Zadawanie obrażeń")]
    public int damageAmount = 10; // Ilość zadawanych obrażeń

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Sprawdzenie, czy pocisk trafił w gracza
        if (collision.collider.CompareTag("Player"))
        {
            if (damageMode)
            {
                // Zadawanie obrażeń graczowi
                PlayerAttacking playerHealth = collision.collider.GetComponent<PlayerAttacking>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamagePlayer(damageAmount);
                }
            }

            if (pushMode)
            {
                // Odpychanie gracza
                Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                    rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
                }
            }
        }

        // Zniszczenie pocisku po kolizji
        Destroy(gameObject);
    }
}
