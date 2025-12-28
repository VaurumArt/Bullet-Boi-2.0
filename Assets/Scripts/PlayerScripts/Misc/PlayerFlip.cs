using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlip : MonoBehaviour
{
    private bool facingRight = true;

    [Header("Flip Settings")]
    public bool autoFlip = true;

    [Header("References")]
    public SpriteRenderer playerSpriteRenderer; // Only flip the player sprite, not the whole transform

    private GunRotation gunRotation;

    void Start()
    {
        // Auto-find player sprite renderer if not assigned
        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Cache gun rotation reference
        gunRotation = GetComponentInChildren<GunRotation>();
    }

    void Update()
    {
        if (!autoFlip) return;

        // Get mouse position from GunRotation if available (more efficient)
        Vector3 mouseWorldPosition;
        if (gunRotation != null)
        {
            mouseWorldPosition = gunRotation.GetMouseWorldPosition();
        }
        else if (Mouse.current != null)
        {
            mouseWorldPosition = GetMouseWorldPosition();
        }
        else
        {
            return;
        }

        UpdateFlip(mouseWorldPosition.x);
    }

    public void UpdateFlip(float mouseXPosition)
    {
        FlipSprite(mouseXPosition);
    }

    void FlipSprite(float mouseXPosition)
    {
        if (playerSpriteRenderer == null) return;

        // Check if the mouse is on the right or left side of the player
        if (mouseXPosition < transform.position.x && facingRight)
        {
            // Mouse is to the left, flip the sprite
            playerSpriteRenderer.flipX = true;
            facingRight = false;
        }
        else if (mouseXPosition > transform.position.x && !facingRight)
        {
            // Mouse is to the right, face right
            playerSpriteRenderer.flipX = false;
            facingRight = true;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        if (Mouse.current == null) return transform.position;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(
            mouseScreenPosition.x,
            mouseScreenPosition.y,
            0f
        ));
        mouseWorldPosition.z = 0f;
        return mouseWorldPosition;
    }

    public bool IsFacingRight()
    {
        return facingRight;
    }

    public void SetFacingDirection(bool faceRight)
    {
        if (playerSpriteRenderer == null) return;

        if (faceRight && !facingRight)
        {
            playerSpriteRenderer.flipX = false;
            facingRight = true;
        }
        else if (!faceRight && facingRight)
        {
            playerSpriteRenderer.flipX = true;
            facingRight = false;
        }
    }
}