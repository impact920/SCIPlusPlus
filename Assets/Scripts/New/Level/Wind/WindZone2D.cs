using System.Collections;
using UnityEngine;

public class WindZone2D : MonoBehaviour
{
    [Header("Wind Settings")]
    public Vector2 windDirection = Vector2.right;
    public float windForce = 10f;

    [Header("Timing")]
    public float windDuration = 2f;   // jak długo wieje
    public float windCooldown = 3f;   // przerwa między podmuchami

    private bool windActive = false;

    private void Start()
    {
        StartCoroutine(WindRoutine());
    }

    private IEnumerator WindRoutine()
    {
        while (true)
        {
            windActive = true;
            yield return new WaitForSeconds(windDuration);

            windActive = false;
            yield return new WaitForSeconds(windCooldown);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!windActive) return;

        Rigidbody2D rb = other.attachedRigidbody;

        if (rb != null)
        {
            rb.AddForce(windDirection.normalized * windForce);
        }
    }
}