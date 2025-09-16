using UnityEngine;
using System.Collections;
public class PLayerInfoScript : MonoBehaviour
{
    [Header("Player Stats")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float iFramesDuration = .5f;
    public float numFlashes = 4f;

    public SpriteRenderer headRend;
    public SpriteRenderer bodyRend;
    [Header("Player Speed")]

 
    [Header("Bullet Upgrades")]
    public float bulletSpeed = 30f;
    //BulletSize Upgrade 
    [Header("Player Upgrades")]
    public float TeleportationCD = 3f;
    //Focus Upgrade 
    //Momentum Scale Upgrade 

    PlayerMovement PlayerMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         PlayerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
    
    }

      public void OnCollisionEnter2D(Collision2D collision)
    {
        

        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            health = health - 10f;
            StartCoroutine(Invunerability());
        }
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
            
        }
    }
    IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(6, 7,true);
        Physics2D.IgnoreLayerCollision(6, 8, true);
        for (int i = 0; i < numFlashes; i++)
        {
        
            bodyRend.color = new Color(1, 0, 0, .5f);
            headRend.color = new Color(1, 0, 0, .5f);
            yield return new WaitForSeconds(iFramesDuration/(numFlashes*2));
       
            bodyRend.color = new Color(1, 0.8f, 0, 1);
            headRend.color = new Color(1, 0.8f, 0, 1);
            yield return new WaitForSeconds(iFramesDuration / (numFlashes * 2));    
        }
        Physics2D.IgnoreLayerCollision(6, 7,false);
        Physics2D.IgnoreLayerCollision(6, 8, false);
    }
}
