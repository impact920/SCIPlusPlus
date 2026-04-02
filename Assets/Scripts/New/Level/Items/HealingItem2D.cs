using UnityEngine;
using System.Collections;

public class HealingItem2D : MonoBehaviour
{
    [Header("Tryb leczenia")]
    public bool infiniteHealing = false; // checkbox

    [Header("Jednorazowe leczenie")]
    public int healAmount = 20;

    [Header("Leczenie w czasie")]
    public float healInterval = 1f;

    [Header("Efekty (opcjonalne)")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        if (infiniteHealing)
        {
            playerHealth.Heal(healAmount);
            new WaitForSeconds(healInterval);
        }
        else
        {
            // jednorazowe leczenie
            playerHealth.Heal(healAmount);
            Destroy(gameObject);
        }

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);

        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

    }

}