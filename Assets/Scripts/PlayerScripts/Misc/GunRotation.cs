using UnityEngine;
using UnityEngine.InputSystem;

public class GunRotation : MonoBehaviour
{
    [Header("Gun Settings")]
    public bool smoothRotation = false;
    public float rotationSpeed = 10f;

    private bool facingRight = true;
    private Camera mainCamera;

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

        // Get the mouse position in screen space using new Input System
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // Convert the screen position to world position
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            mouseScreenPosition.x,
            mouseScreenPosition.y,
            mainCamera.nearClipPlane
        ));

        // For 2D games, set Z to match gun's Z position
        mouseWorldPosition.z = transform.position.z;

        // Calculate the direction from the gun to the mouse position
        Vector2 direction = (mouseWorldPosition - transform.position).normalized;

        // Calculate the angle to rotate the gun towards the mouse
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

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


    // Optional: Get mouse world position (useful for other scripts)
    public Vector3 GetMouseWorldPosition()
    {
        if (Mouse.current == null) return Vector3.zero;

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            mouseScreenPosition.x,
            mouseScreenPosition.y,
            mainCamera.nearClipPlane
        ));

        mouseWorldPosition.z = transform.position.z;
        return mouseWorldPosition;
    }

    // Optional: Check if mouse button is pressed
    public bool IsMousePressed()
    {
        return Mouse.current != null && Mouse.current.leftButton.isPressed;
    }

    // Optional: Check if mouse button was clicked this frame
    public bool IsMouseClicked()
    {
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    }
}