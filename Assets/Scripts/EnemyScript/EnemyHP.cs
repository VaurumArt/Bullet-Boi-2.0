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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreSystemScript = FindAnyObjectByType<ScoreSystemScript>();
        enemyParryBehaviour = GetComponent<EnemyParryBehaviour>();
        currentHealth = maxHealth;

<<<<<<< HEAD
<<<<<<< HEAD
=======
        gameManager.EnemyAlive();

=======
      
>>>>>>> parent of 3d36f30 (Win and lose con)

        if (bloodTransform == null)
            Debug.LogError("BloodTransform is NOT assigned!", this);
>>>>>>> recovered
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
        if(collision.gameObject.CompareTag("Player"))
        {
       Rigidbody2D playerRb =collision.gameObject.GetComponent<Rigidbody2D>();
          
            float playerSpeed = playerRb.linearVelocity.magnitude;
            PlayerMovement playermovment = collision.gameObject.GetComponent<PlayerMovement>();
            if (!playermovment.isKnockedBack && playerSpeed >= deathSpeed) //Check if the player speed is fast enough to kill the enemy 
            {
                TakeDamage(20);
            }

        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletScript bulletInfo = collision.gameObject.GetComponent<BulletScript>();
            TakeDamage(bulletInfo.bulletDamage);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
           if(enemyParryBehaviour.isParried)
            {
                TakeDamage(5);
            }
        }


    }
    public void Die()
    {
        deathFeedback?.PlayFeedbacks();
        scoreSystemScript.BiteKillScore();
        Debug.Log("Basic Enemy Died");
        Instantiate(bloodSplatter, bloodTransform.position, bloodTransform.rotation);
        Destroy(gameObject);
    }


}
