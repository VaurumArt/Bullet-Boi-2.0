using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public float maxHealth = 10f;
    public float currentHealth;
    public float lethalSpeed = 30f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
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

        currentHealth = currentHealth - damage;
     
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
       Rigidbody2D playerRb =collision.gameObject.GetComponent<Rigidbody2D>();
          
            float playerSpeed = playerRb.linearVelocity.magnitude;

            if (playerSpeed >= lethalSpeed) //Check if the player speed is fast enough to kill the enemy 
            {
             
                Die();
            }

        }
        if (collision.gameObject.CompareTag("Bullet"))
        {

            Die();

        }
    }
    public void Die()
    {
        Debug.Log("Basic Enemy Died");
        Destroy(gameObject);
    }
}
