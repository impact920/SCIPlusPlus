using UnityEngine;

public class HealingItem2D : MonoBehaviour
{
    [Header("Ustawienia leczenia")]
    public int healAmount = 20;

    [Header("Efekty (opcjonalne)")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        playerHealth.Heal(healAmount);

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }
}
