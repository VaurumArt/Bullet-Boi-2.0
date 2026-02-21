using UnityEngine;

public class BouncerEnemy : MonoBehaviour
{
    [Header("Enemy Movement")]
    public float minAngle = 0.25f;
    public float maxAngle = 0.25f;
    public float dirX = 1;
    public float dirY = 0.25f;
    public float speed;
    public GameObject rightCheck, leftCheck, roofCheck, groundCheck;
    public LayerMask groundLayer;
    [Header("Collider size")]
    public Vector2 rightBox = new Vector2(2, 2);
    public Vector2 leftBox = new Vector2(2, 2);
    public Vector2 roofBox = new Vector2(2, 2);
    public Vector2 floorBox = new Vector2(2, 2);

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D enemyRb;
    private float lastBounceTime;
    private bool groundTouch, roofTouch, rightTouch, leftTouch;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyRb = GetComponent<Rigidbody2D>();
        dirX = Random.Range(minAngle, maxAngle) * (Random.value < 0.5f ? -1 : 1);
        dirY = Random.value < 0.5f ? -1 : 1;
    }

    void Update()
    {
        enemyRb.linearVelocity = new Vector2(dirX, dirY) * speed;
        SpriteFlip();
        HitDetection();
    }

    void SpriteFlip()
    {
        if (enemyRb.linearVelocity.x < 0)
            spriteRenderer.flipX = true;
        else if (enemyRb.linearVelocity.x > 0)
            spriteRenderer.flipX = false;
    }

    void HitDetection()
    {
        rightTouch = Physics2D.OverlapBox(rightCheck.transform.position, rightBox, 0f, groundLayer);
        leftTouch = Physics2D.OverlapBox(leftCheck.transform.position, leftBox, 0f, groundLayer);
        roofTouch = Physics2D.OverlapBox(roofCheck.transform.position, roofBox, 0f, groundLayer);
        groundTouch = Physics2D.OverlapBox(groundCheck.transform.position, floorBox, 0f, groundLayer);
        HitLogic();
    }

    void HitLogic()
    {
        if (Time.time - lastBounceTime < 0.1f) return;

        if (rightTouch)
        {
            dirX = -Random.Range(minAngle, maxAngle);
            lastBounceTime = Time.time;
        }
        else if (leftTouch)
        {
            dirX = Random.Range(minAngle, maxAngle);
            lastBounceTime = Time.time;
        }

        if (roofTouch)
        {
            dirY = -1;
            lastBounceTime = Time.time;
        }
        else if (groundTouch)
        {
            dirY = 1;
            lastBounceTime = Time.time;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(rightCheck.transform.position, rightBox);
        Gizmos.DrawWireCube(leftCheck.transform.position, leftBox);
        Gizmos.DrawWireCube(roofCheck.transform.position, roofBox);
        Gizmos.DrawWireCube(groundCheck.transform.position, floorBox);
    }
}