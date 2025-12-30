using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public GameObject bloodSplatter;
    public Transform bloodTransform;
    
    public float maxHealth = 10f;
    public float currentHealth;
    public float deathSpeed = 30f;

    public Color normalColor = Color.white;
    public Color damagedColor = Color.red;
    public float damageFrame = 0.5f;
    public SpriteRenderer enemyBody; 

    ScoreSystemScript scoreSystemScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreSystemScript = FindAnyObjectByType<ScoreSystemScript>();
     
        currentHealth = maxHealth;

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
        StartCoroutine(DamageIndicator());
        currentHealth = currentHealth - damage;
       

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
       Rigidbody2D playerRb =collision.gameObject.GetComponent<Rigidbody2D>();
          
            float playerSpeed = playerRb.linearVelocity.magnitude;

            if (playerSpeed >= deathSpeed) //Check if the player speed is fast enough to kill the enemy 
            {
                TakeDamage(50);
            }

        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletScript bulletInfo = collision.gameObject.GetComponent<BulletScript>();
            TakeDamage(bulletInfo.bulletDamage);
        }
        
        
    }
    public void Die()
    {
        scoreSystemScript.BiteKillScore();
        Debug.Log("Basic Enemy Died");
        Instantiate(bloodSplatter, bloodTransform.position, bloodTransform.rotation);
        Destroy(gameObject);
    }

    IEnumerator DamageIndicator()
    {
        enemyBody.color = damagedColor;
        yield return  new WaitForSeconds(damageFrame);
        enemyBody.color = normalColor;

    }
}
