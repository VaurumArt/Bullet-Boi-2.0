using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyFollower : MonoBehaviour
{
    public float speed;
    public float lineOfSight = 20;
    private Transform player;
    public Vector2 randPos;
    public bool isChasing = false;

    //EnemyFlip
    private float playerpos;
    public float patrolCooldown = 3f;
    private bool facingRight = true;

    // Patrol system
    private bool hasRandomPos = false;
    private bool isPatrolling = false;
    public float maxRangeX = 10;
    public float minRangeX = 10;
    public float maxRangeY = 10;
    public float minRangeY = 10;
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        StartCoroutine(PatrolSystem());
    }

    void Update()
    {
        playerpos = transform.position.x - player.position.x;
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceFromPlayer < lineOfSight)
        {
            // Chase player
            isChasing = true;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            FlipTowards(player.position); // Flip towards player
        }
        else
        {
            // Patrol mode
            isChasing = false;

            if (hasRandomPos)
            {
                GoToPos();
            }
        }
    }

    private void GoToPos()
    {
        if (hasRandomPos)
        {
            // Move towards random position
            transform.position = Vector2.MoveTowards(transform.position, randPos, speed * Time.deltaTime);
            FlipTowards(randPos); // Flip towards patrol point

            // Check if reached destination
            if (Vector2.Distance(transform.position, randPos) < 0.1f)
            {
                hasRandomPos = false; // Arrived, get new position
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Cast transform.position to Vector2
            Vector2 awayFromWall = ((Vector2)transform.position - collision.contacts[0].point).normalized;

            // Generate new position away from the wall
            float randX = transform.position.x + (awayFromWall.x * Random.Range(minRangeX, maxRangeX));
            float randY = transform.position.y + (awayFromWall.y * Random.Range(minRangeY, maxRangeY));

            randPos = new Vector2(randX, randY);
            hasRandomPos = true;
        }
    }
    private IEnumerator PatrolSystem()
    {
        while (true) // Run forever
        {
            if (!isChasing && !hasRandomPos)
            {
                RandomPosGenerator();
                hasRandomPos = true;
            }

            yield return new WaitForSeconds(patrolCooldown);
        }
    }

    private void RandomPosGenerator()
    {
        float randX = transform.position.x + Random.Range(minRangeX, maxRangeX);
        float randY = transform.position.y + Random.Range(minRangeY, maxRangeY);
        randPos = new Vector2(randX, randY);
    }

    void FlipTowards(Vector2 target)
    {
        float direction = target.x - transform.position.x;

        if (direction > 0 && !facingRight) // Moving right, facing left
        {
            Flip();
        }
        else if (direction < 0 && facingRight) // Moving left, facing right
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
    }
}