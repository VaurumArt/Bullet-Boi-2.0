using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 40f;

    private Vector2 lookDirection;
    private float lookAngle;
    private GameObject lastBullet;

    void Update()
    {
        UpdateAimDirection();
    }

    private void UpdateAimDirection()
    {
        // Get mouse position and convert to world space
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        // Calculate look direction and angle
        lookDirection = (mouseWorldPos - transform.position).normalized;
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        // Rotate fire point to aim direction
        firePoint.rotation = Quaternion.Euler(0, 0, lookAngle);
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        //// Destroy previous bullet if it exists
        //if (lastBullet != null)
        //{
        //    Destroy(lastBullet);
        //}

        // Instantiate new bullet
        lastBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Apply velocity to bullet
        Rigidbody2D bulletRb = lastBullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = lookDirection * bulletSpeed;
        }
    }

    // Public getter for teleport script to access last bullet
    public GameObject GetLastBullet()
    {
        return lastBullet;
    }

    // Public method for teleport script to clear bullet reference
    public void ClearLastBullet()
    {
        lastBullet = null;
    }
}