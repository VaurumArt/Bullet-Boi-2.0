using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootAndTP : MonoBehaviour
{
    [Header("Shooting")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed =40f;
    public float momentumMulti = 0.5f; //Bullet momemtun to Player's Momentum Scaler
    private GameObject lastBullet;
    Vector2 lookDirection;
    float lookAngle;
    
    public float teleportMomentumDuration = 4f; // How long momentum lasts
  
    private bool isTeleporting = false; // Flag to disable movement control
    private Rigidbody2D rb;
    PlayerHP playerHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHP = GetComponent<PlayerHP>();
        rb = GetComponent<Rigidbody2D>(); // Fixed: assign to class field, not local variable
    }

    // Update is called once per frame
    void Update()
    {
        // Use the new Input System to get mouse position
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWolrdPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWolrdPos.z = 0;

        lookDirection = (mouseWolrdPos - transform.position).normalized;
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, lookAngle);
    }

    #region SHOOT
    public void OnShoot(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            //Debug.Log("Fire!");
            playerHP.bulletHealthDrain();
            GameObject bulletClone = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, lookAngle));
            Rigidbody2D rbBullet = bulletClone.GetComponent<Rigidbody2D>();
            rbBullet.linearVelocity= lookDirection * bulletSpeed;

            lastBullet = bulletClone;
        }

    }

    #endregion

    public void OnTeleport(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (lastBullet != null)
            {
                //Get the player current speed before Teleporting and add it when teleporting to sta
                Vector2 playerPreviousSpeed = rb.linearVelocity;
                // Get bullet components FIRST (before destroying!)
                Rigidbody2D bulletRb = lastBullet.GetComponent<Rigidbody2D>();

                // Get bullet's velocity BEFORE destroying it
                Vector2 bulletVelocity = bulletRb.linearVelocity;
                float combinedSpeed;

                // Get the Speeds (magnitude) 
                float playerSpeed = playerPreviousSpeed.magnitude;
                float bulletSpeed = bulletVelocity.magnitude;

                if (bulletSpeed > playerSpeed)
                {
                    combinedSpeed = (bulletSpeed * momentumMulti);
                    // apply combined speed in bullet's direction 
                    Vector2 bulletDirection = bulletVelocity.normalized;
                    rb.linearVelocity = bulletDirection * combinedSpeed;
                }
                else if (bulletSpeed < playerSpeed)
                {
                    combinedSpeed = (playerSpeed);
                    // if player is faster than bullet the momentum will be kept 
                    Vector2 bulletDirection = bulletVelocity.normalized;
                    rb.linearVelocity = bulletDirection * combinedSpeed;
                }
                // Teleport to bullet position
                transform.position = lastBullet.transform.position;
                // Set teleport flag and start momentum timer
                isTeleporting = true;
                StartCoroutine(EndTeleportMomentum());

                // NOW destroy the bullet and clear reference (AFTER getting velocity!)
                Destroy(lastBullet);
                lastBullet = null;
            }
            else
            {
               // Debug.Log("No bullet available");
            }
        }


    }

    // Debug coroutine to check if velocity is being overridden
IEnumerator CheckVelocityOverTime()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForFixedUpdate();
           // Debug.Log($"Frame {i}: Player velocity = {rb.linearVelocity}");
        }
    }

    // Coroutine to end teleport momentum after duration
IEnumerator EndTeleportMomentum()
    {
        yield return new WaitForSeconds(teleportMomentumDuration);
        isTeleporting = false;
        //Debug.Log("Teleport momentum ended - movement control restored");
    }
}
