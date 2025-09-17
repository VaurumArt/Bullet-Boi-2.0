using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class EnemyBullet : MonoBehaviour
{
    Rigidbody2D bulletRb;
    GameObject target; 
    public float speed;
    private void Start()
    {
        bulletRb =GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");
        Vector2 moveDir = (target.transform.position - transform.position).normalized * speed;
        bulletRb.linearVelocity = new Vector2(moveDir.x,moveDir.y);
        Destroy(this.gameObject, 2); //destroy in 2 sec
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall" )|| collision.gameObject.CompareTag( "Player"))
        {
            Destroy(gameObject);
        }
    }
}