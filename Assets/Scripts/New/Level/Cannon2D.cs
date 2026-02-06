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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Ustaw kierunek
        float direction = shootRight ? 1f : -1f;
        bullet.transform.right = new Vector2(direction, 0f);

        // Przekazanie prędkości do pocisku
        EnemyBullet2D bulletScript = bullet.GetComponent<EnemyBullet2D>();
        if (bulletScript != null)
        {
            bulletScript.speed = bulletSpeed;
        }
    }
}
