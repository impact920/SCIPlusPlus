using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Ustawienia czasu")]
    public float timeBeforeFall = 1f; // Czas przed rozpoczęciem upadku
    public float timeBeforeReset = 5f; // Czas przed resetem platformy (opcjonalnie)

    private Vector3 initialPosition; // Pozycja początkowa platformy
    private Quaternion initialRotation; // Obrót początkowy platformy
    private Rigidbody2D rb; // Komponent Rigidbody2D
    private bool isFalling = false; // Czy platforma spada?
    private bool playerTouched = false; // Czy gracz dotknął platformy?

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Brak komponentu Rigidbody2D! Dodaj go do obiektu.");
            return;
        }

        rb.isKinematic = true; // Ustaw platformę jako nieruchomą na start
        initialPosition = transform.position; // Zapamiętaj pozycję początkową
        initialRotation = transform.rotation; // Zapamiętaj obrót początkowy
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !playerTouched)
        {
            playerTouched = true; // Oznacz, że gracz dotknął platformy
            StartCoroutine(StartFalling());
        }
    }

    private IEnumerator StartFalling()
    {
        isFalling = true;
        yield return new WaitForSeconds(timeBeforeFall);

        rb.isKinematic = false; // Platforma zaczyna spadać
        rb.gravityScale = 1f; // Ustaw grawitację dla platformy

        yield return new WaitForSeconds(timeBeforeReset);

        if (isFalling) // Upewniamy się, że platforma opadła
        {
            ResetPlatform();
        }
    }

    private void ResetPlatform()
    {
        rb.isKinematic = true; // Ustaw platformę jako nieruchomą
        rb.linearVelocity = Vector2.zero; // Zatrzymaj ruch
        rb.angularVelocity = 0f; // Zatrzymaj obrót
        transform.position = initialPosition; // Przywróć pozycję początkową
        transform.rotation = initialRotation; // Przywróć obrót początkowy
        isFalling = false; // Resetuj stan
        playerTouched = false; // Resetuj informację o kontakcie gracza
    }
}
