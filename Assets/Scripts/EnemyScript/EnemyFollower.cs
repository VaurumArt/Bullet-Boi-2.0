using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class EnemyFollower : MonoBehaviour
{
    public float speed;
    public float lineOfSight = 20;
    private Transform player;
   
    //EnemyFlip
    private float playerpos;
    private bool facingRight=true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        playerpos =  transform.position.x - player.position.x ;//subtract the postion of the player relative to the enemy 
        float distanceFromPlayer = Vector2.Distance( player.position, transform.position);
       if (distanceFromPlayer < lineOfSight)
        {
       
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, speed * Time.deltaTime);
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
    }

}
