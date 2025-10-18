using UnityEngine;
using System.Collections;

public class GravityFlipper : MonoBehaviour
{
    [Header("Czas przełączania")]
    public float flipCooldown = 1f; // Czas między kolejnymi zmianami grawitacji
    private bool canFlip = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Jeżeli obiekt wejdzie w obszar i jest to gracz
        if (other.CompareTag("Player") && canFlip)
        {
            FlipGravity(other);  // Przekazujemy collider gracza
            StartCoroutine(FlipCooldown());
        }
    }

    private void FlipGravity(Collider2D playerCollider)
    {
        Rigidbody2D playerRigidbody = playerCollider.GetComponent<Rigidbody2D>(); // Pobieramy Rigidbody2D gracza

        if (playerRigidbody != null)
        {
            // Odwrócenie kierunku grawitacji
            playerRigidbody.gravityScale *= -1;

            // Obrót gracza o 180 stopni
            Vector3 newRotation = playerRigidbody.transform.eulerAngles;
            newRotation.z += 180f; // Obrót wizualny obiektu
            playerRigidbody.transform.eulerAngles = newRotation;

            Debug.Log("Grawitacja gracza odwrócona!");
        }
    }

    private IEnumerator FlipCooldown()
    {
        canFlip = false;
        yield return new WaitForSeconds(flipCooldown);
        canFlip = true;
    }
}
