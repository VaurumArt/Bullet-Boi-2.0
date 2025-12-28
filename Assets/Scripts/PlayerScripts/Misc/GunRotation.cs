using UnityEngine;
using UnityEngine.InputSystem;

public class GunRotation : MonoBehaviour
{
    [Header("Gun Settings")]
    public bool smoothRotation = false;
    public float rotationSpeed = 10f;
    public float rotationOffset = 0f;

    [Header("Sprite Settings")]
    public SpriteRenderer gunSpriteRenderer;

    private Camera mainCamera;
    private Vector3 mouseWorldPosition;
    private PlayerFlip playerFlip;

    void Start()
    {
        // Cache all references once
        mainCamera = Camera.main;
        playerFlip = GetComponentInParent<PlayerFlip>();

        if (gunSpriteRenderer == null)
        {
            gunSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Get mouse position
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0f));
        mouseWorldPosition.z = 0f;

        // Calculate direction and angle
        Vector2 direction = mouseWorldPosition - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;

        // Apply rotation
        if (smoothRotation)
        {
            float smoothedAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        }

        // Flip gun sprite when aiming left (between 90° and 270°)
        if (gunSpriteRenderer != null)
        {
            gunSpriteRenderer.flipY = targetAngle > 90f || targetAngle < -90f;
        }

        // Notify player to flip (one call per frame)
        if (playerFlip != null)
        {
            playerFlip.UpdateFlip(mouseWorldPosition.x);
        }
    }

    // Efficient getters (return cached values)
    public Vector3 GetMouseWorldPosition() => mouseWorldPosition;
    public float GetLookAngle() => transform.eulerAngles.z;
    public Vector2 GetLookDirection()
    {
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    // Input checks
    public bool IsMousePressed() => Mouse.current != null && Mouse.current.leftButton.isPressed;
    public bool IsMouseClicked() => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
}