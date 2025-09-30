using UnityEngine;
using UnityEngine.InputSystem;

public class GunRotation : MonoBehaviour
{
    [Header("Gun Settings")]
    public bool smoothRotation = false;
    public float rotationSpeed = 10f;
    public float rotationOffset = 0f; // Adjust if sprite doesn't face right by default

    private Camera mainCamera;
    private Vector3 cachedMouseWorldPosition;

    void Start()
    {
        // Cache camera reference for better performance
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }

    void Update()
    {
        // Check if mouse is available
        if (Mouse.current == null) return;

        // Calculate and cache mouse world position
        cachedMouseWorldPosition = CalculateMouseWorldPosition();

        // Calculate the direction from the gun to the mouse position
        Vector2 direction = (cachedMouseWorldPosition - transform.position).normalized;

        // Calculate the angle to rotate the gun towards the mouse
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;

        // Apply rotation (smooth or instant)
        if (smoothRotation)
        {
            // Smooth rotation
            float currentAngle = transform.eulerAngles.z;
            float smoothedAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, smoothedAngle);
        }
        else
        {
            // Instant rotation
            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
        }
    }

    // Calculate mouse world position (called once per frame)
    private Vector3 CalculateMouseWorldPosition()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // For orthographic camera, Z value doesn't affect the result
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            mouseScreenPosition.x,
            mouseScreenPosition.y,
            0f
        ));

        mouseWorldPosition.z = 0f; // Keep it at 0 for 2D
        return mouseWorldPosition;
    }

    // Get cached mouse world position (efficient for other scripts to use)
    public Vector3 GetMouseWorldPosition()
    {
        return cachedMouseWorldPosition;
    }

    // Get the current look angle (useful for bullet spawning)
    public float GetLookAngle()
    {
        return transform.eulerAngles.z;
    }

    // Get the direction the gun is facing
    public Vector2 GetLookDirection()
    {
        float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    // Check if mouse button is pressed
    public bool IsMousePressed()
    {
        return Mouse.current != null && Mouse.current.leftButton.isPressed;
    }

    // Check if mouse button was clicked this frame
    public bool IsMouseClicked()
    {
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    }
}