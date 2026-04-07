using UnityEngine;

public class DamageDealer2D : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;
    public float damageInterval = 1f;

    private PlayerHealth playerHealth;
    private float nextDamageTime = 0f;
    private bool playerInside = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        playerHealth = null;
    }

    private void Update()
    {
        if (!playerInside || playerHealth == null) return;

        if (Time.time >= nextDamageTime)
        {
            playerHealth.TakeDamage(damage);
            nextDamageTime = Time.time + damageInterval;
        }
    }
}