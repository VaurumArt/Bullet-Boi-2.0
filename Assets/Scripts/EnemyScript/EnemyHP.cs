using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;
public class EnemyHP : MonoBehaviour
{
    [Header("FeedBacks")]
    public MMFeedbacks deathFeedback;
    public MMFeedbacks damageFeedback;
    public GameObject bloodSplatter;
    public Transform bloodTransform;
    
    public float maxHealth = 10f;
    public float currentHealth;
    public float deathSpeed = 30f;

   
    public float damageFrame = 0.5f;
 

    ScoreSystemScript scoreSystemScript;
    EnemyParryBehaviour enemyParryBehaviour;
    GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
       
    }
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();  
        scoreSystemScript = FindAnyObjectByType<ScoreSystemScript>();
        enemyParryBehaviour = GetComponent<EnemyParryBehaviour>();
        currentHealth = maxHealth;

        gameManager.EnemyAlive();


        if (bloodTransform == null)
            Debug.LogError("BloodTransform is NOT assigned!", this);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage( float damage)
    {
        damageFeedback?.PlayFeedbacks();
        currentHealth = currentHealth - damage;
       

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out Rigidbody2D playerRb) &&
                collision.gameObject.TryGetComponent(out PlayerMovement playerMovement))
            {
                float playerSpeed = playerRb.linearVelocity.magnitude;

                if (!playerMovement.isKnockedBack && playerSpeed >= deathSpeed)
                {
                    TakeDamage(20);
                }
            }
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.TryGetComponent(out BulletScript bulletInfo))
            {
                TakeDamage(bulletInfo.bulletDamage);
            }
        }

        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Enemy"))
        {
            if (enemyParryBehaviour != null && enemyParryBehaviour.isParried)
            {
                TakeDamage(2);
            }
        }
    }

    public void Die()
    {
        gameManager.EnemyDeath();
        deathFeedback?.PlayFeedbacks();
        scoreSystemScript.BiteKillScore();
        Debug.Log("Basic Enemy Died");
        Instantiate(bloodSplatter, bloodTransform.position, bloodTransform.rotation);
        Destroy(gameObject);
    }


}
