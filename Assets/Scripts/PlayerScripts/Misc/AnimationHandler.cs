using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator body;
    [SerializeField] private Animator head;
    [SerializeField] private Animator parry;
    private PlayerMovement playerMovement;

    [Header("Jump/Fall Settings")]
    [SerializeField] private float apexThreshold = 0.1f; // small deadzone for vertical velocity

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void FixedUpdate()
    {
        JumpAndFallCheck();
    }

    #region Jump & Fall

    private void JumpAndFallCheck()
    {
        if (playerMovement == null || body == null) return;

        float yVel = playerMovement.rb.linearVelocity.y;

        // Grounded state
        if (playerMovement.isGrounded)
        {
            SetBoolSafe(body, "IsGrounded", true);
            SetBoolSafe(body, "IsRising", false);
            SetBoolSafe(body, "IsFalling", false);
            return;
        }

        // Air state
        SetBoolSafe(body, "IsGrounded", false);

        if (yVel > apexThreshold)
        {
            // Rising
            SetBoolSafe(body, "IsRising", true);
            SetBoolSafe(body, "IsFalling", false);
        }
        else if (yVel < -apexThreshold)
        {
            // Falling
            SetBoolSafe(body, "IsRising", false);
            SetBoolSafe(body, "IsFalling", true);
        }
        else
        {
            // Apex / zero velocity
            SetBoolSafe(body, "IsRising", false);
            SetBoolSafe(body, "IsFalling", false);
        }
    }

    #endregion

    #region Bite Animations

    public void BiteAnimationOn()
    {
        if (head != null)
            SetBoolSafe(head, "IsBiting", true);
    }

    public void BiteAnimationOff()
    {
        if (head != null)
            SetBoolSafe(head, "IsBiting", false);
    }

    #endregion

    #region Utility

    // Only update Animator if value changed to prevent unnecessary calls
    private void SetBoolSafe(Animator anim, string param, bool value)
    {
        if (anim != null && anim.GetBool(param) != value)
        {
            anim.SetBool(param, value);
        }
    }

    #endregion

    #region Parry Animations
    public void ParryAnimationOn()
    {
        if (parry != null)
            SetBoolSafe(parry, "IsParrying", true);
    }
    public void ParryAnimationOff()
    {
        if (parry != null)
            SetBoolSafe(parry, "IsParrying",false);
    }
    #endregion
}
