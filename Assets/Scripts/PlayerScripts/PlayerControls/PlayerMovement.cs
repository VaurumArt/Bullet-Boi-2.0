using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Header("Bite Speed Up")]
    public float biteSpeedUpMult = 0.3f;
    public float playerSpeed;

    [Header("Movements")]
    public float moveSpeed = 25.0f;
    public float moveAirSpeed = 5.0f;
    public float requiredSpeed = 20f;
    public float jumpForce = 5f;
    private bool isBraked;

    [Header("Ground Checker")]
    public bool isGrounded;// cannot be private other script is accessing it 
    public Transform groundCheck;
    public LayerMask groundLayers;
    public float groundCheckRadius = .5f;

    [HeaderAttribute("Trails")]
    public GameObject fast;
    public GameObject faster;
    public GameObject fastest;
    public GameObject meteoric;
    public GameObject devine;

    public float knockbackForce = 10f;
    public float knockbackDuration = 0.3f;

    public bool isKnockedBack = false;
    private GameObject currentActiveTrail;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    public Vector2 lookDirection;

    WallBounceScript wallBounceScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trailDisabler();
        rb = GetComponent<Rigidbody2D>();
        wallBounceScript = GetComponent<WallBounceScript>();
    }

    // Update is called once per frame
    void Update()
    {
        BrakeCheck();
        GroundCheck();

        if (isGrounded && isBraked && !isKnockedBack)
        {
            if (moveInput.x != 0) // Only change velocity when pressing movement keys
            {
                rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
            }
            else
            {
                // Apply friction/slowdown when not moving
                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x * 0.95f, // Adjust this value (0.9 - 0.98)
                    rb.linearVelocity.y
                );
            }
        }

        playerSpeed = rb.linearVelocity.magnitude;
        TrailChecker();
        LookDirection();
    }

    public void BiteSpeedup()
    {
        rb.linearVelocity = rb.linearVelocity + (rb.linearVelocity * biteSpeedUpMult); // add .3 of speed to the player after biting an enemy
    }

    public void PlayerKnockBack(Vector2 direction)
    {
if (!isKnockedBack)
        {
            StartCoroutine(KnockBackCoroutine(direction));
                   
        }
    }

    private IEnumerator KnockBackCoroutine(Vector2 direction)
    {
        isKnockedBack = true;

        // Add upward component for arc effect
        Vector2 knockback = new Vector2(direction.x, 0.5f).normalized * knockbackForce;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y); // Slow down
        isKnockedBack = false;
    }
    void trailDisabler()
    {
        fast.SetActive(false);
        faster.SetActive(false);
        fastest.SetActive(false);
        meteoric.SetActive(false);
        devine.SetActive(false);

    }

    void BrakeCheck()
    {
        // add an animation that the character is trying to counter act the speed 
        float currentSpeed = rb.linearVelocity.magnitude;
        isBraked = currentSpeed <= requiredSpeed;
    }

    void GroundCheck()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);
    }
    #region PLAYER_MOVEMENT
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

    }


    public void LookDirection()
    {
        // Use the new Input System to get mouse position
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWolrdPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWolrdPos.z = 0;

        lookDirection = (mouseWolrdPos - transform.position).normalized;
    }
    #endregion
    void TrailChecker()
    {
        GameObject trailTarget = null;

        if (playerSpeed >= 150f)
        {
            trailTarget = devine;
        }
        else if (playerSpeed >= 120f)
        {
            trailTarget = meteoric;
        }
        else if (playerSpeed >= 90f)
        {
            trailTarget = fastest;
        }
        else if (playerSpeed >= 60f)
        {
            trailTarget = faster;
        }
        else if (playerSpeed >= 30f)
        {

            trailTarget = fast;
        }
      
        if (trailTarget != currentActiveTrail)
        {
            if (currentActiveTrail != null)
            {
                currentActiveTrail.SetActive(false);
            }

            if (trailTarget != null)
            {
                trailTarget.SetActive(true);
            }
            currentActiveTrail = trailTarget;
        }
    }
      


}
