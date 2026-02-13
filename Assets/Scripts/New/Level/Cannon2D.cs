using UnityEngine;

public class Cannon2D : MonoBehaviour
{
    [Header("Strzelanie")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;
    public float bulletSpeed = 8f;

    [Header("Kierunek")]
    public bool shootRight = true;

    private float fireTimer;

    void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            Shoot();
            fireTimer = 0f;
        }
    }

    void Shoot()
    {
        // Tworzymy pocisk
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Ustawiamy kierunek
        Vector2 direction = shootRight ? Vector2.right : Vector2.left;
        bullet.transform.right = direction;

        // Inicjalizujemy pocisk
        EnemyBullet2D bulletScript = bullet.GetComponent<EnemyBullet2D>();
        if (bulletScript != null)
        {
            bulletScript.Init(bulletSpeed, direction);
        }
    }
}
