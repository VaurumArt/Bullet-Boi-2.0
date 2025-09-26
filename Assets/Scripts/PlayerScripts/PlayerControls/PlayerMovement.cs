using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    [Header("Movement")]
    private Vector2 moveInput;
    public float moveSpeed = 5.0f;
    private Rigidbody2D rb;
    public float requiredSpeed = 10f;
    public bool isBraked;
    public float jumpForce = 5f;


    [Header("Ground Checker")]
    public bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayers;
    public float groundCheckRadius = .5f;

    [HeaderAttribute("Trails")]
    public GameObject fast;
    public GameObject faster;
    public GameObject fastest;
    public GameObject meteoric;
    public GameObject devine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trailDisabler();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {


        BrakeCheck();
        GroundCheck();
        //&& isBraked
        if (isGrounded ) //Check if Player isGrounded and current speed is lower that required speed the player can move 
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }


        playerSpeed = rb.linearVelocity.magnitude;
        TrailChecker();
    }

    public void BiteSpeedup()
    {
        rb.linearVelocity = rb.linearVelocity + (rb.linearVelocity * .3f); // add .3 of speed to the player after biting an enemy
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
    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            Debug.Log("Jump");
            rb.AddForce(Vector2.up * jumpForce);    
        }

    }

    #endregion
    async Task TrailChecker()
    {

        if (playerSpeed >= 150f)
        {
            fast.SetActive(false);
            faster.SetActive(false);
            fastest.SetActive(false);
            meteoric.SetActive(false);
            devine.SetActive(true);
        }
        else if (playerSpeed >= 120f)
        {
            fast.SetActive(false);
            faster.SetActive(false);
            fastest.SetActive(false);
            meteoric.SetActive(true);
        }
        else if (playerSpeed >= 90f)
        {
            fast.SetActive(false);
            faster.SetActive(false);
            fastest.SetActive(true);
        }
        else if (playerSpeed >= 60f)
        {
            fast.SetActive(false);
            faster.SetActive(true);
        }
        else if (playerSpeed >= 30f)
        {

            fast.SetActive(true);
        }
        else if (playerSpeed < 30f)
        {
            trailDisabler();
        }
    }


}
