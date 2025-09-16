using UnityEngine;
using UnityEngine.UIElements;

public class BouncerEnemy : MonoBehaviour
{
    [Header("Enemy Movement")]
    public float Angle = 0.25f;
    public float dirX = 1;
    public float dirY = 0.25f;
    public float speed;
    public GameObject rightCheck, roofCheck, groundCheck;
    public LayerMask groundLayer;

    [Header("Collider size")]
    public Vector2 rightBox = new Vector2 (2,2);
    public Vector2 roofBox = new Vector2(2, 2);
    public Vector2 floorBox = new Vector2(2, 2);
   

    private Rigidbody2D enemyRb;
    private bool facingRight = true;
    private bool groundTouch;
    private bool roofTouch;
    private bool rightTouch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyRb.linearVelocity = new Vector2(dirX, dirY) * speed; // * Time.deltaTime;
        HitDetection();
    }
    void HitDetection()
    {
        rightTouch = Physics2D.OverlapBox(rightCheck.transform.position, rightBox, groundLayer);
        roofTouch = Physics2D.OverlapBox(roofCheck.transform.position, roofBox, groundLayer);
        groundTouch = Physics2D.OverlapBox(groundCheck.transform.position, floorBox, groundLayer);
        HitLogic();
    }

    void HitLogic()
    {
        if (rightTouch && facingRight)
        {
            Flip();
        }
        else if (rightTouch && !facingRight)
        {
            Flip();
        }
        if (roofTouch)
        {
            dirY = -Angle;
        }
        else if (groundTouch)
        {
            dirY = Angle;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(new Vector3(0, 180, 0));
        dirX = -dirX;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(rightCheck.transform.position, rightBox);
        Gizmos.DrawWireCube(roofCheck.transform.position, roofBox);
        Gizmos.DrawWireCube(groundCheck.transform.position, floorBox);
    }
}



