using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallBounceTimeBasedScript : MonoBehaviour
{
    [Header("Wall Bounce Detection")]
    public Transform PlayerCenter;
    public float range = 3f;
    public LayerMask WallLayers;

    [Header("Wall Bounce Check")]
    public bool isBouncing = false;
    public bool canBounce = true;
    public Color bounceColor;
    public Color activeBounceColor = Color.purple;
    public Color inActiveBounceColor = Color.blue;
    public Color CooldownBounceColor = Color.yellow;

    [Header("Bounce Duration & CD")]
    public float bounceReadyDuration = 0.5f;
    public float bounceCooldown = 0.5f;
    public float bounceTickRate = 0.05f;

    [Header("Reaction Time")]
    public float perfectReaction = 0.2f;
    public float goodReaction = 0.5f;
    public float perfectBounce = 1.25f;
    public float goodBounce = 1.0f;
    public float lateBounce = 0.5f;
    public float jumpForce = 750;

    PlayerMovement playerMovement;
    Rigidbody2D rb;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        bounceColor = inActiveBounceColor;
    }

    public void OnJump()
    {
        if (!isBouncing && canBounce && playerMovement.playerSpeed >= 24)
        {
            Debug.Log("Bouncing!");
            StartCoroutine(BounceActivated());
        }
        else if (!canBounce)
        {
            Debug.Log("Bounce is on Cooldown");
        }
        else if (playerMovement.isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
            Debug.Log("Jump");
        }
    }

    bool PerformBounce(float reactionTime)
    {
        float currentSpeed = playerMovement.playerSpeed;
        Collider2D wallcheck = Physics2D.OverlapCircle(PlayerCenter.position, range, WallLayers);

        if (!wallcheck)
        {
            Debug.Log("No Wall Detected!");
            return false;
        }

        float multiplier;
        if (reactionTime < perfectReaction)
        {
            Debug.Log("PERFECT");
            multiplier = perfectBounce;
        }
        else if (reactionTime < goodReaction)
        {
            Debug.Log("Good");
            multiplier = goodBounce;
        }
        else
        {
            Debug.Log("Late");
            multiplier = lateBounce;
        }

        float newSpeed = currentSpeed * multiplier;
        rb.linearVelocity = playerMovement.lookDirection * newSpeed;
        return true;   
    }

    IEnumerator BounceActivated()
    {
        isBouncing = true;
        bounceColor = activeBounceColor;
        float elapsedTime = 0;

        while (elapsedTime < bounceReadyDuration)
        {
         bool bounceSuccessful =  PerformBounce(elapsedTime);
            if (bounceSuccessful)
            {
                break;
            }
 
            yield return new WaitForSeconds(bounceTickRate);
            elapsedTime += bounceTickRate;
        }

        isBouncing = false;
        bounceColor = inActiveBounceColor;
        StartCoroutine(BounceIsCoolingDown());
    }

    IEnumerator BounceIsCoolingDown()
    {
        canBounce = false;
        bounceColor = CooldownBounceColor;
        yield return new WaitForSeconds(bounceCooldown);
        canBounce = true;
        bounceColor = inActiveBounceColor;
        Debug.Log("Bounce is Available");
    }

    private void OnDrawGizmos()
    {
        if (PlayerCenter == null) return;
        Gizmos.color = bounceColor;
        Gizmos.DrawWireSphere(PlayerCenter.position, range);
    }
}