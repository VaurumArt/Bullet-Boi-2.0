using UnityEngine;

public class EnemyAutoShoot : MonoBehaviour
{
    [Header("Setup")]
    public GameObject bulletPrefab;
    public Transform[] shootingPoints;

    [Header("Settings")]
    public float fireRate = 1f;
    public float bulletSpeed = 5f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        foreach (Transform point in shootingPoints)
        {
            if (point == null) continue;

            GameObject bullet = Instantiate(bulletPrefab, point.position, point.rotation);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = point.right * bulletSpeed;
            }
        }
    }
}