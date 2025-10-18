using UnityEngine;

public class Canon : MonoBehaviour
{
    [Header("Ustawienia strzelania")]
    public GameObject bulletPrefab; // Prefab pocisku
    public Transform firePoint; // Punkt wystrzału
    public float fireRate = 1f; // Częstotliwość strzelania (pociski na sekundę)
    public float bulletSpeed = 10f; // Prędkość pocisku

    [Header("Tryb działania pocisku")]
    public bool damageMode = true; // True = zadawanie obrażeń, False = brak obrażeń
    public bool pushMode = true; // True = odpychanie, False = brak odpychania
    public float pushForce = 5f; // Siła odpychania

    [Header("Kierunek strzału")]
    public bool shootLeft = false; // True = strzał w lewo, False = strzał w prawo

    private float nextFireTime = 0f;

    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Fire()
    {
        // Tworzenie pocisku
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Wyznacz kierunek strzału na podstawie zmiennej shootLeft
            Vector2 shootDirection = shootLeft ? -firePoint.right : firePoint.right;
            rb.linearVelocity = shootDirection * bulletSpeed;
        }

        // Ustaw tryby pocisku
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.damageMode = damageMode;
            bulletScript.pushMode = pushMode;
            bulletScript.pushForce = pushForce;
        }
    }
}
