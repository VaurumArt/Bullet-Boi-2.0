using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float momentumMultiplier = 0.5f;
    public float teleportMomentumDuration = 4f;

    private Rigidbody2D rb;
    private PlayerShooting playerShooting;
    private bool isTeleporting = false;
    public float teleportCooldown =1.5f;
    private bool canTeleport = true; 
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerShooting = GetComponent<PlayerShooting>();

        if (playerShooting == null)
        {
            Debug.LogError("PlayerShooting component not found!");
        }
    }

    public void OnTeleport(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (canTeleport)
            {
                TryTeleport();
            }
          
        }
    }

   public  IEnumerator TeleportCooldown()
    {   canTeleport = false;
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }
    private void TryTeleport()
    {

        GameObject lastBullet = playerShooting.GetLastBullet();

        if (lastBullet == null)
        {
            Debug.Log("No bullet available to teleport to");
            return;
        }

        // Store player's current velocity
        Vector2 playerPreviousVelocity = rb.linearVelocity;

        // Get bullet's rigidbody and velocity BEFORE destroying
        Rigidbody2D bulletRb = lastBullet.GetComponent<Rigidbody2D>();
        if (bulletRb == null)
        {
            Debug.LogError("Bullet has no Rigidbody2D!");
            return;
        }

        Vector2 bulletVelocity = bulletRb.linearVelocity;
        Vector2 bulletDirection = bulletVelocity.normalized;

        // Calculate momentum transfer
        float playerSpeed = playerPreviousVelocity.magnitude;
        float bulletSpeed = bulletVelocity.magnitude;
        float combinedSpeed = CalculateCombinedSpeed(playerSpeed, bulletSpeed);

        // Teleport to bullet position
        transform.position = lastBullet.transform.position;

        // Apply momentum in bullet's direction
        rb.linearVelocity = bulletDirection * combinedSpeed;

        // Start momentum timer
        isTeleporting = true;
        StartCoroutine(EndTeleportMomentum());

        // Destroy bullet and clear reference
        Destroy(lastBullet);
        playerShooting.ClearLastBullet();
        StartCoroutine(TeleportCooldown());
    }

    private float CalculateCombinedSpeed(float playerSpeed, float bulletSpeed)
    {
        if (bulletSpeed > playerSpeed)
        {
            // Bullet is faster: use scaled bullet speed
            return bulletSpeed * momentumMultiplier;
        }
        else
        {
            // Player is faster: maintain player speed
            return playerSpeed;
        }
    }

    private IEnumerator EndTeleportMomentum()
    {
        yield return new WaitForSeconds(teleportMomentumDuration);
        isTeleporting = false;
        Debug.Log("Teleport momentum ended - movement control restored");
    }

    // Public getter for other scripts to check teleport state
    public bool IsTeleporting()
    {
        return isTeleporting;
    }
}

