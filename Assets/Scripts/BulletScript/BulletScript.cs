using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class BulletScript : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float bulletDuration = 3f;
    public float ricochetCount = 0f; // number of bullet bounce or ricochet. 
    public float requiredRicochet = 0f; // number of ricochet to be destory.

  //  [Header("Ricochet Settings")]
   // public bool maintainSpeed = true; // If true, bullet keeps same speed after ricochet
    //public float speedDecayPerRicochet = 0.9f; // Speed reduction per ricochet (if maintainSpeed is false)

    [Header("Optional Effects")]
    public GameObject sparkEffect; // Assign spark effect prefab
    public AudioClip ricochetSound; // Assign ricochet sound

    private Rigidbody2D bulletRb;
    private AudioSource audioSource;
    private float originalSpeed;
    public float speed = 50f; 
    void Start()
    {
        
        bulletRb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        if (bulletRb != null)
        {
            originalSpeed = bulletRb.linearVelocity.magnitude;

            // Apply impulse once
            BulletForce();
            originalSpeed = speed;
        }
    }
      


    

    void Update()
    {

        bulletDuration -= Time.deltaTime;
        if (bulletDuration < 0)
        {
            DestroyBullet();
        }
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") )     
        {
           if (ricochetCount >= requiredRicochet)
            {
                DestroyBullet();
            }
           else
            {
                ricochetCount++;
            }

          
        }

        if ( collision.gameObject.CompareTag("Enemy"))
        {
            DestroyBullet();
        }
    }

    public void BulletForce()
    {
        bulletRb.AddForce(transform.right * speed , ForceMode2D.Impulse);
    }
  
    
}