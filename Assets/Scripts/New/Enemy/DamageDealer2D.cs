using UnityEngine;

public class DamageDealer2D : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 10;
    public float damageInterval = 1f;

    private PlayerHealth playerHealth;
    private float nextDamageTime = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (playerHealth == null)
            playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null) return;

        // ⏱️ Sprawdzamy czy minął cooldown
        if (Time.time >= nextDamageTime)
        {
            playerHealth.TakeDamage(damage);
            nextDamageTime = Time.time + damageInterval;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = null;
        }
    }
}