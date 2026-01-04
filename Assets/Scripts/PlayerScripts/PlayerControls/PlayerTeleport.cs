using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float momentumMultiplier = 0.5f;
    public float teleportMomentumDuration = 4f;
    public float teleportCooldown = 1.5f;

    [Header("Speed Accumulation")]
    [Tooltip("Minimum speed for first teleport (when starting from zero)")]
    public float minimumTeleportSpeed = 35f;
    [Tooltip("How much of the bullet's speed to add to current speed")]
    public float speedAdditionMultiplier = 0.3f;
    [Tooltip("Maximum speed you can reach through teleporting")]
    public float maxTeleportSpeed = 100f;

    private Rigidbody2D rb;
    private PlayerShooting playerShooting;
    private bool isTeleporting = false;
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

    public IEnumerator TeleportCooldown()
    {
        canTeleport = false;
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

        // Store player's current velocity (this is your accumulated speed!)
        Vector2 playerCurrentVelocity = rb.linearVelocity;
        float currentSpeed = playerCurrentVelocity.magnitude;

        // Get bullet's rigidbody and velocity BEFORE destroying
        Rigidbody2D bulletRb = lastBullet.GetComponent<Rigidbody2D>();
        if (bulletRb == null)
        {
            Debug.LogError("Bullet has no Rigidbody2D!");
            return;
        }

        Vector2 bulletVelocity = bulletRb.linearVelocity;
        Vector2 bulletDirection = bulletVelocity.normalized;
        float bulletSpeed = bulletVelocity.magnitude;

        // Calculate NEW speed = current speed + portion of bullet speed
        float speedToAdd = bulletSpeed * speedAdditionMultiplier;
        float newSpeed = currentSpeed + speedToAdd;

        // If starting from zero or very low speed, use minimum base speed
        if (newSpeed < minimumTeleportSpeed)
        {
            newSpeed = minimumTeleportSpeed;
        }

        // Cap at maximum speed
        newSpeed = Mathf.Min(newSpeed, maxTeleportSpeed);

        Debug.Log($"Teleport: Current speed: {currentSpeed:F1} + Added: {speedToAdd:F1} = New speed: {newSpeed:F1}");

        // Teleport to bullet position
        transform.position = lastBullet.transform.position;

        // Apply accumulated speed in bullet's direction
        rb.linearVelocity = bulletDirection * newSpeed;

        // Reset momentum timer
        if (isTeleporting)
        {
            StopCoroutine("EndTeleportMomentum");
        }

        isTeleporting = true;
        StartCoroutine(EndTeleportMomentum());

        // Destroy bullet and clear reference
        Destroy(lastBullet);
        playerShooting.ClearLastBullet();
        StartCoroutine(TeleportCooldown());
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