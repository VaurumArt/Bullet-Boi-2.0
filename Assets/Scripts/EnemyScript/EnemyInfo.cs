using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public float contactDamage = 10;
    public float stopForce = 10f;
    public float parrySpeed= 10f;
    public Rigidbody2D rbEnemy;

    [Header("Parry State")]
    public bool isParried = false;
    public float parryStunDuration = 1f;

    [Header("Script to Disable on Parry")]
    [Tooltip("Drag the movement script here (can be different for each enemy)")]
    public MonoBehaviour movementScript;

    void Start()
    {
        rbEnemy = GetComponent<Rigidbody2D>();
    }

    public void GetParried()
    {
        isParried = true;

        // Disable the specific movement script
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        Invoke(nameof(RecoverFromParry), parryStunDuration);
    }

    void RecoverFromParry()
    {
        isParried = false;

        // Re-enable the movement script
        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
    }
}