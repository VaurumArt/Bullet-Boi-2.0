using UnityEngine;

public class PlayerFlip : MonoBehaviour
{
    private bool facingRight = true;

    [Header("Flip Settings")]
    public bool autoFlip = true; // If false, only flips when called by GunRotation

    void Update()
    {
        // Only auto-flip if enabled and no GunRotation is controlling it
        if (autoFlip)
        {
            // This method is kept for backward compatibility
            // But it's better to let GunRotation control the flipping
            GunRotation gunRotation = FindObjectOfType<GunRotation>();
            if (gunRotation == null)
            {
                // Fallback to manual mouse tracking if no GunRotation found
                Vector3 mouseWorldPosition = GetMouseWorldPositionFallback();
                UpdateFlip(mouseWorldPosition.x);
            }
        }
    }

    // Called by GunRotation to update flip state
    public void UpdateFlip(float mouseXPosition)
    {
        FlipSprite(mouseXPosition);
    }

    void FlipSprite(float mouseXPosition)
    {
        // Check if the mouse is on the right or left side of the player
        if (mouseXPosition < transform.position.x && facingRight)
        {
            // Mouse is to the left, flip the sprite
            transform.localScale = new Vector3(-1f, 1f, 1f); // Flip on the X-axis
            facingRight = false;
        }
        else if (mouseXPosition > transform.position.x && !facingRight)
        {
            // Mouse is to the right, set the sprite to face right
            transform.localScale = new Vector3(1f, 1f, 1f); // Reset scale
            facingRight = true;
        }
    }

    // Fallback method using old Input system (for backward compatibility)
    Vector3 GetMouseWorldPositionFallback()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = 0.1f;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        return mouseWorldPosition;
    }

    // Public method to check facing direction
    public bool IsFacingRight()
    {
        return facingRight;
    }

    // Public method to manually set facing direction
    public void SetFacingDirection(bool faceRight)
    {
        if (faceRight && !facingRight)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            facingRight = true;
        }
        else if (!faceRight && facingRight)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            facingRight = false;
        }
    }
}