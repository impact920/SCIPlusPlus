using System.Collections;
using UnityEngine;

public class DamageDealer2D : MonoBehaviour
{
    [Header("Ustawienia obrażeń")]
    public int damage = 10;
    public float damageInterval = 1f; // co ile sekund zadaje obrażenia

    private bool playerInside = false;
    private PlayerHealth playerHealth;
    private Coroutine damageCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerInside = true;
                damageCoroutine = StartCoroutine(DamageOverTime());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (damageCoroutine != null)
                StopCoroutine(damageCoroutine);
        }
    }

    IEnumerator DamageOverTime()
    {
        while (playerInside)
        {
            playerHealth.TakeDamage(damage);
            yield return new WaitForSeconds(damageInterval);
        }
    }
}
