using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject wave;
    public Transform teleportDestination; // Miejsce, do którego gracz zostanie przeniesiony

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sprawdzamy, czy obiekt, który wchodzi w portal, ma tag "Player"
        if (other.CompareTag("Player") && teleportDestination != null)
        {
            // Przenieœ gracza do ustalonej pozycji
            other.transform.position = teleportDestination.position;
            wave.transform.position = teleportDestination.position - new Vector3(20,0, 0 );
        }
    }
}
