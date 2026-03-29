using UnityEngine;

public class Segment : MonoBehaviour
{
    [Tooltip("Długość segmentu w jednostkach świata (np. szerokość tilemapy).")]
    public float segmentLength = 10f;

    [Tooltip("Czy segment został już załadowany do gry?")]
    public bool isLoaded = false;

    private void OnDrawGizmosSelected()
    {
        // Rysuje pomocniczą linię długości segmentu w scenie
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * segmentLength);
    }
}
