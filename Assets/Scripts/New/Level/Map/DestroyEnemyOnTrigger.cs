using UnityEngine;

public class DestroyEnemyOnTrigger : MonoBehaviour
{
    // Funkcja wywoływana gdy coś wejdzie do triggera
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sprawdzamy, czy obiekt ma tag "Enemy"
        if (other.CompareTag("Enemy"))
        {
            // Usuwamy przeciwnika z gry
            Destroy(other.gameObject);
        }
    }
}