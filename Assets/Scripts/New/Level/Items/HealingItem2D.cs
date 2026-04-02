using UnityEngine;
using System.Collections;

public class HealingItem2D : MonoBehaviour
{
    [Header("Tryb leczenia")]
    public bool infiniteHealing = false;

    [Header("Jednorazowe leczenie")]
    public int healAmount = 20;

    [Header("Leczenie w czasie")]
    public float healInterval = 1f;

    [Header("Efekty (opcjonalne)")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private Coroutine healCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        if (infiniteHealing)
        {
            // start leczenia w pętli
            if (healCoroutine == null)
                healCoroutine = StartCoroutine(HealOverTime(playerHealth));
        }
        else
        {
            // jednorazowe leczenie
            playerHealth.Heal(healAmount);

            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
            healCoroutine = null;
        }
    }

    private IEnumerator HealOverTime(PlayerHealth playerHealth)
    {
        while (true)
        {
            playerHealth.Heal(healAmount);
            yield return new WaitForSeconds(healInterval);
        }
    }
}