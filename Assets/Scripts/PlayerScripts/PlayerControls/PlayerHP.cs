using UnityEngine;
using System.Collections;
public class PlayerHP : MonoBehaviour
{
    [Header("Player Stats")]
    public float health = 100f;
    public float maxHealth = 100f;
    [Header("Bullet Drain")]
    public float bulletHpDrain = 10;
    public float biteHealUp = 100;
    [Header("Enemy Collision Heal")]
    public float collisionHealUp = 10;
    [Header("Damage Indicator")]
    public float invulnerableFramesDuration = .5f;
    public float numFlashes = 4f;

    public SpriteRenderer headRend;
    public SpriteRenderer bodyRend;
    PlayerMovement PlayerMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
      health = Mathf.Clamp(health, 0, maxHealth);
    }

      public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            health = health - 10f;
            StartCoroutine(Invunerability());
        }
    }

    public void bulletHealthDrain()
    {
        health -= bulletHpDrain;
    }

    public void BiteHeal()
    {
        health += biteHealUp;
    }

    public void CollisionHeal()
    {
        
        health += collisionHealUp;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") )
        {
            if (PlayerMovement.playerSpeed < 30)
            {
                health = health - 10f;
                StartCoroutine(Invunerability());
            }
            else
            { 
            CollisionHeal();
            }
            
        }
    }
    IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(6, 7,true);
        Physics2D.IgnoreLayerCollision(6, 10, true);
        for (int i = 0; i < numFlashes; i++)
        {
        
            bodyRend.color = new Color(1, 0, 0, .5f);
            headRend.color = new Color(1, 0, 0, .5f);
            yield return new WaitForSeconds(invulnerableFramesDuration / (numFlashes*2));
       
            bodyRend.color = new Color(1, 0.8f, 0, 1);
            headRend.color = new Color(1, 0.8f, 0, 1);
            yield return new WaitForSeconds(invulnerableFramesDuration / (numFlashes * 2));    
        }
        Physics2D.IgnoreLayerCollision(6, 7,false);
        Physics2D.IgnoreLayerCollision(6, 10, false);
    }
}
