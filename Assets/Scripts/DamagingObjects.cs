using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObjects : MonoBehaviour
{
    public int damageToPlayer = 10;
    public int staminaToPlayer = 10; // Obra¿enia zadawane graczowi
    public LayerMask playerLayers; // Warstwa gracza
    public float damageInterval = 1f; // Odstêp czasu pomiêdzy obra¿eniami w sekundach

    private float nextDamageTime = 0f; // Kiedy mo¿na zadaæ kolejne obra¿enia
    private Collider2D playerInZone; // Gracz w strefie obra¿eñ
    public Animator animator; // Opcjonalny animator do animacji

    private void Update()
    {
        // Jeœli gracz jest w strefie, zadaj obra¿enia w okreœlonych odstêpach czasu
        if (playerInZone != null && Time.time >= nextDamageTime)
        {
            DealDamageToPlayer(playerInZone);
            nextDamageTime = Time.time + damageInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sprawdzamy, czy obiekt jest graczem
        if (((1 << other.gameObject.layer) & playerLayers) != 0)
        {
            playerInZone = other;
            nextDamageTime = Time.time; // Reset czasu, aby obra¿enia zadane by³y natychmiast
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Jeœli gracz opuœci strefê, przestajemy zadawaæ obra¿enia
        if (playerInZone == other)
        {
            playerInZone = null;
        }
    }

    private void DealDamageToPlayer(Collider2D player)
    {
        // Próba zadania obra¿eñ graczowi
        PlayerAttacking playerScript = player.GetComponent<PlayerAttacking>();
        PlayerMovement playerscript = player.GetComponent<PlayerMovement>();
        if (playerScript != null)
        {
            playerScript.TakeDamagePlayer(damageToPlayer);
            playerscript.UseStamina(staminaToPlayer);


        }
    }

    private void OnDrawGizmos()
    {
        // Rysowanie obszaru dzia³ania w edytorze (np. gdyby obiekt mia³ obszar dzia³ania w colliderze)
        Gizmos.color = Color.red;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
