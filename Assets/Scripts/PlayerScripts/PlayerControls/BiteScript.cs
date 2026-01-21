using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using MoreMountains.Feedbacks;
public class BiteScript : MonoBehaviour
{
    public Animator animator;
    [SerializeField] ScoreSystemScript ScoreScript;

    [Header("FeedBack")]

    public MMFeedbacks BiteFeedBack;
    public MMFeedbacks BiteSuccessFeedBack;

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
    public float biteCooldown = 0.42f;
    public float biteDamage = 50f;
    public float biteTickRate = 0.05f;
    public float bitePause = 0.75f;
    public float bitePauseFactor = 0;

    PlayerHP playerHP;
    PlayerMovement playerMovement;
    SlowMoScript SlowMoScript;
    AnimationHandler animationHandler;
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
        if (!isBiting && canBite)// Check if the player is already pressing the control and check if the cooldown of bite is ready
        {
            //   Debug.Log("Bite!");

            StartCoroutine(BiteActivated());
        }
        else if (!canBite)
        {
            //   Debug.Log("Bite is on cooldown!");
        }

    }

    // Function to check if the player already did the action. 
    //  Function that change the color of the gizmo for better debugging 
    IEnumerator BiteActivated()
    {
        BiteFeedBack?.PlayFeedbacks();
        BiteColor = activeBiteColor;
        isBiting = true;
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
        StartCoroutine(BiteCoolingDown());
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
        //  Debug.Log("Bite tick!");
        Vector3 offsetPosition = attackPoint.position + attackPoint.right * (biteSize.x * 0.5f);
        Collider2D[] bitEnemies = Physics2D.OverlapBoxAll(offsetPosition, biteSize, attackPoint.eulerAngles.z, EnemyLayers);
        foreach (Collider2D enemy in bitEnemies)
        {
            //  Debug.Log("We hit " + enemy.name);
            EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();
            if (enemyHP != null)
            {
                StartCoroutine(BitePauseTime());
                BiteSuccessFeedBack?.PlayFeedbacks();
                enemyHP.TakeDamage(biteDamage);
                playerMovement.BiteSpeedup();
                playerHP.BiteHeal();
                ScoreScript.BiteKillScore();
            }
            else
            {
                //  Debug.Log("error no enemy");
            }
        }
    }
    //Function controls the cooldown of the Ability 
    //Fucntion change the color of the gizmo to yellow to show that it's cooling down 
    IEnumerator BiteCoolingDown()
    {
        canBite = false;
        BiteColor = cooldownBiteColor;
        yield return new WaitForSeconds(biteCooldown);
        BiteColor = inActiveBiteColor;
        canBite = true;
        // Debug.Log("Bite is Available");
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = BiteColor;
        Vector2 forwardOffset = new Vector2(biteSize.x * 0.5f, 0);
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            attackPoint.position +
           (Vector3)(Quaternion.Euler(0, 0, attackPoint.eulerAngles.z) * forwardOffset),
            Quaternion.Euler(0, 0, attackPoint.eulerAngles.z),
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, biteSize);
        Gizmos.matrix = oldMatrix;
    }
}
