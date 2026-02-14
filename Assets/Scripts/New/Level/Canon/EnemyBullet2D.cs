using UnityEngine;

public class EnemyBullet2D : MonoBehaviour
{
    [Header("Ustawienia")]
    public int damage = 10;
    public float speed = 10f;
    public float lifeTime = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Metoda do inicjalizacji pocisku
    public void Init(float bulletSpeed, Vector2 direction)
    {
        speed = bulletSpeed;
        rb.linearVelocity = direction * speed;

        // Zniszcz pocisk po określonym czasie
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
