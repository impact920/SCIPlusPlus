using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObjects : MonoBehaviour
{
    public int damageToPlayer = 10;
    public LayerMask playerLayers; // Warstwa gracza
    public float damageInterval = 1f; // Odst�p czasu pomi�dzy obra�eniami w sekundach

    private float nextDamageTime = 0f; // Kiedy mo�na zada� kolejne obra�enia
    private Collider2D playerInZone; // Gracz w strefie obra�e�
    public Animator animator; // Opcjonalny animator do animacji

    private void Update()
    {
        // Je�li gracz jest w strefie, zadaj obra�enia w okre�lonych odst�pach czasu
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
            nextDamageTime = Time.time; // Reset czasu, aby obra�enia zadane by�y natychmiast
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Je�li gracz opu�ci stref�, przestajemy zadawa� obra�enia
        if (playerInZone == other)
        {
            playerInZone = null;
        }
    }

    private void DealDamageToPlayer(Collider2D player)
    {
        // Pr�ba zadania obra�e� graczowi
        PlayerAttacking playerScript = player.GetComponent<PlayerAttacking>();
        PlayerMovement playerscript = player.GetComponent<PlayerMovement>();
        if (playerScript != null)
        {
            playerScript.TakeDamagePlayer(damageToPlayer);


        }
    }

    private void OnDrawGizmos()
    {
        // Rysowanie obszaru dzia�ania w edytorze (np. gdyby obiekt mia� obszar dzia�ania w colliderze)
        Gizmos.color = Color.red;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }
}
