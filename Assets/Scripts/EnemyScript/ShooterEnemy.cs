using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ShooterEnemy : MonoBehaviour
{
    public float fireRate = 1.0f;
    public float nextFireTime;
    public float speed;
    public float lineOfSight = 20;
    public float shootingRange = 15f;
    private Transform player;
    public GameObject bulletParent;
    public GameObject bullets;
    //EnemyFlip
    private float playerpos;
    private bool facingRight = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        playerpos = transform.position.x - player.position.x;//subtract the postion of the player relative to the enemy 

        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);
        if (distanceFromPlayer < lineOfSight && distanceFromPlayer > shootingRange)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
            FlipEnemy();

        }
        else if (distanceFromPlayer <= lineOfSight && nextFireTime < Time.time)
        {
           
            Instantiate(bullets,bulletParent.transform.position,Quaternion.identity);
            nextFireTime = Time.time + fireRate;
   
        }

        if (distanceFromPlayer <= lineOfSight)
        {
            FlipEnemy();
        }
    }
    void FlipEnemy()
    {
        if (playerpos < 0 && !facingRight)
        {// the player is in the right 
            facingRight = true;
            transform.Rotate(new Vector3(0, -180, 0));
        }
        else if (playerpos > 0 && facingRight)
        {
            facingRight = false;
            // the player is in the left 
            transform.Rotate(new Vector3(0, 180, 0));
        }



    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, lineOfSight);
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }

}
