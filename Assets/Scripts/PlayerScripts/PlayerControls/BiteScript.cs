using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class BiteScript : MonoBehaviour
{
    public Animator animator;
    [SerializeField] ScoreSystemScript ScoreScript;

    [Header("FeedBack")]
    public MMFeedbacks biteFeedBack;
    public MMFeedbacks biteSuccessFeedBack;
    public MMFeedbacks biteCdFeedback;

    [Header("Bite Dectection HitBox")]
    private bool isBiting = false;
    private bool canBite = true;
    private Color BiteColor;
    public Color activeBiteColor = Color.green;
    public Color inActiveBiteColor = Color.red;
    public Color cooldownBiteColor = Color.yellow;

    [Header("Bite Detection")]
    public Transform attackPoint;
    public Vector2 biteSize = new Vector2(2, 2);
    public LayerMask EnemyLayers;

    [Header("Bite Duration&Cooldown")]
    public float biteDuration = 0.3f;
    public float biteCooldown = 0.7f;
    public float biteHitCooldown = 0.3f;
    public float biteDamage = 50f;
    public float biteTickRate = 0.05f;
    public float bitePause = 0.75f;
    public float bitePauseFactor = 0;

    PlayerHP playerHP;
    PlayerMovement playerMovement;
    SlowMoScript SlowMoScript;
    AnimationHandler animationHandler;

    // Track which enemies were hit during this bite to avoid multiple hits
    private HashSet<Collider2D> hitEnemiesThisBite = new HashSet<Collider2D>();
    private bool hitAnyEnemyThisBite = false;

    private void Start()
    {
        animationHandler = GetComponent<AnimationHandler>();
        SlowMoScript = GetComponent<SlowMoScript>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHP = GetComponent<PlayerHP>();
        BiteColor = inActiveBiteColor;
    }

    public void OnBite()
    {
        if (!isBiting && canBite)
        {
            StartCoroutine(BiteActivated());
        }
    }

    IEnumerator BiteActivated()
    {
        biteFeedBack?.PlayFeedbacks();
        BiteColor = activeBiteColor;
        isBiting = true;
        hitAnyEnemyThisBite = false;
        hitEnemiesThisBite.Clear();

        float elapsedTime = 0f;
        while (elapsedTime < biteDuration)
        {
            PerformBite();
            animationHandler.BiteAnimationOn();
            yield return new WaitForSeconds(biteTickRate);
            elapsedTime += biteTickRate;
        }

        animationHandler.BiteAnimationOff();
        BiteColor = inActiveBiteColor;
        isBiting = false;

        // Pass whether we hit an enemy to the cooldown
        StartCoroutine(BiteCoolingDown(hitAnyEnemyThisBite));
    }

    IEnumerator BitePauseTime()
    {
        SlowMoScript.TimeStopper();
        Time.timeScale = bitePauseFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        yield return new WaitForSecondsRealtime(bitePause);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        SlowMoScript.TimeResume();
    }

    void PerformBite()
    {
        Vector3 offsetPosition = attackPoint.position + attackPoint.right * (biteSize.x * 0.5f);
        Collider2D[] bitEnemies = Physics2D.OverlapBoxAll(offsetPosition, biteSize, attackPoint.eulerAngles.z, EnemyLayers);

        foreach (Collider2D enemy in bitEnemies)
        {
            // Only damage each enemy once per bite activation
            if (hitEnemiesThisBite.Contains(enemy))
                continue;

            EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();
            if (enemyHP != null)
            {
                // Mark this enemy as hit
                hitEnemiesThisBite.Add(enemy);
                hitAnyEnemyThisBite = true;

                // Trigger effects only on first enemy hit this bite
                if (hitEnemiesThisBite.Count == 1)
                {
                    StartCoroutine(BitePauseTime());
                    biteSuccessFeedBack?.PlayFeedbacks();
                    playerMovement.BiteSpeedup();
                    playerHP.BiteHeal();
                    ScoreScript.BiteKillScore();
                }

                // Damage the enemy
                enemyHP.TakeDamage(biteDamage);
            }
        }
    }

    IEnumerator BiteCoolingDown(bool hitEnemy)
    {
        canBite = false;
        BiteColor = cooldownBiteColor;

        // No cooldown if we hit an enemy, otherwise use normal cooldown
        float cooldownTime;

        if (hitEnemy)
        {
            cooldownTime = biteHitCooldown; // Instant recast
        }
        else
        {
            cooldownTime = biteCooldown; // Normal cooldown (0.42s)
        }

        yield return new WaitForSeconds(cooldownTime);

        biteCdFeedback?.PlayFeedbacks();
        BiteColor = inActiveBiteColor;
        canBite = true;
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.color = BiteColor;
        Vector2 forwardOffset = new Vector2(biteSize.x * 0.5f, 0);
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            attackPoint.position + (Vector3)(Quaternion.Euler(0, 0, attackPoint.eulerAngles.z) * forwardOffset),
            Quaternion.Euler(0, 0, attackPoint.eulerAngles.z),
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, biteSize);
        Gizmos.matrix = oldMatrix;
    }
}