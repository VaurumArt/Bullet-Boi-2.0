using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallBounceScript : MonoBehaviour
{
    [Header("Wall Bounce Detection")]
    public float wallDetectionRange = 10f;
    public LayerMask bouncebleLayers;
 

    [Header("Distance Treshholds")]
    public float perfectDistanceMin = 3f;   // Was 2f
    public float perfectDistanceMax = 6f;   // Was 3f (now 3 unit window)
    public float goodDistanceMin = 6f;      // Was 3f
    public float goodDistanceMax = 10f;     // Was 5f (now 4 unit window)
    public float lateDistanceMin = 10f;     // Was 5f
    public float lateDistanceMax = 15f;     //
    [Header("Momentum Multiplier")]
    public float perfectMultiplier = 1.25f;
    public float goodMultiplier = 1.0f;
    public float lateMultiplier = 0.7f;


    [Header("Requirment")]
    public float jumpForce = 750f;
    public float minSpeedforBounce;

    [Header("CoolDown")]
    public float bounceCooldown = 0.5f;

    Rigidbody2D rb;
    PlayerMovement PlayerMovement;
    private bool bounceWindowActive = false;
    private float anticipatedDistance;
    private bool canBounce = true;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerMovement = GetComponent<PlayerMovement>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (PlayerMovement.playerSpeed >= minSpeedforBounce && !PlayerMovement.isGrounded && canBounce)
            {
                Vector2 movementDirection = rb.linearVelocity.normalized;
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,
                    movementDirection,
                    wallDetectionRange,
                    bouncebleLayers
                    );
                if (hit.collider != null)
                {
                    anticipatedDistance = hit.distance;
                    bounceWindowActive = true;
                    Debug.Log($"Wall Bounce armed Distance:{anticipatedDistance:F2}units");
                    }
                else
                {
                    Debug.Log("No Wall Detected ahead");
                }
            }
            else if (PlayerMovement.isGrounded)
            {
                rb.AddForce(Vector2.up * jumpForce);
                Debug.Log("Jump");
            }
            else if (!canBounce)
            {
                Debug.Log("Wall Bounce is on Cooldown");
            }
            
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (bounceWindowActive && IsWallLayer(collision.gameObject))
        {
            PerformWallBounce();
        }
        
    }
    void PerformWallBounce()
    {
        bounceWindowActive = false;
        float currentSpeed = PlayerMovement.playerSpeed;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mousePos);
        mouseWorld.z = 0;
        Vector2 bounceDirection = (mouseWorld - transform.position).normalized;

        float multilpier;
        string quality;

        if (anticipatedDistance >= perfectDistanceMin && anticipatedDistance <= perfectDistanceMax)
        {
            multilpier = perfectMultiplier;
            quality = "Perfect";
        }
        else if (anticipatedDistance >= goodDistanceMin && anticipatedDistance <= goodDistanceMax)
        {
            multilpier = goodMultiplier;
            quality = "GOOD";
        }
        else if (anticipatedDistance >= lateDistanceMin && anticipatedDistance <= lateDistanceMax)
        {
            multilpier = lateMultiplier;
            quality = "LATE";
        }
        else
        {
            multilpier = 0.5f;
            quality = "FAILED";
        }
        float newSpeed = currentSpeed * multilpier;
        rb.linearVelocity = bounceDirection * newSpeed;


        Debug.Log($"{quality}BOUNCE! DISTANCE:{anticipatedDistance:F2}|Speed:{currentSpeed}->{newSpeed:F1}");

            StartCoroutine(BounceCooldown());

    }
    IEnumerator BounceCooldown() 
    {
        canBounce = false;
        yield return new WaitForSeconds(bounceCooldown);
        canBounce = true;
    }
    bool IsWallLayer (GameObject obj)
    {
        return ((1 << obj.layer) & bouncebleLayers) != 0;
    }
    private void OnDrawGizmos()
    {
        if (rb != null && PlayerMovement != null && PlayerMovement.playerSpeed >= minSpeedforBounce )
        {
            Vector2 direction = rb.linearVelocity.normalized;
            Vector3 start = transform.position;
            Vector3 end = start + (Vector3)(direction * wallDetectionRange);

            Gizmos.color = bounceWindowActive ? Color. green: Color.yellow;
            Gizmos.DrawLine(start, end);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(start + (Vector3)(direction * 2.5f), 0.5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(start + (Vector3)(direction * 4f), 0.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(start + (Vector3)(direction * 6f), 0.5f);
        }
    }
}
