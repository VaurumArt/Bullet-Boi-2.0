using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ShooterEnemy : MonoBehaviour
{
    public float fireRate = 1.0f;
    public float nextFireTime;

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
         //   transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
        

        }
        else if (distanceFromPlayer <= lineOfSight && nextFireTime < Time.time)
        {
           
            Instantiate(bullets,bulletParent.transform.position,Quaternion.identity);
            nextFireTime = Time.time + fireRate;
   
        }

      
    }



   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireSphere(transform.position, lineOfSight);
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }

}
