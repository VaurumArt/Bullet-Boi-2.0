using UnityEngine;
using UnityEngine.UIElements;
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
    public Vector2 rightBox = new Vector2 (2,2);
    public Vector2 leftBox = new Vector2(2, 2);
    public Vector2 roofBox = new Vector2(2, 2);
    public Vector2 floorBox = new Vector2(2, 2);

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D enemyRb;
    private float lastBounceTime;
    private bool groundTouch;
    private bool roofTouch;
    private bool rightTouch;
    private bool leftTouch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on " + gameObject.name);
        }
        enemyRb = GetComponent<Rigidbody2D>();
        dirX = Random.Range(minAngle, maxAngle) * (Random.value < 0.5 ? -1 : 1);
        dirY =  Random.value < 0.5 ? -1 : 1;
    }

    // Update is called once per frame
    void Update()
    {
        enemyRb.linearVelocity = new Vector2(dirX, dirY) * speed; // * Time.deltaTime;

        HitDetection();
    }
    void HitDetection()
    {
        rightTouch = Physics2D.OverlapBox(rightCheck.transform.position, rightBox,0f, groundLayer);
        leftTouch = Physics2D.OverlapBox(leftCheck.transform.position, leftBox, 0f, groundLayer);
        roofTouch = Physics2D.OverlapBox(roofCheck.transform.position, roofBox, 0f, groundLayer);
        groundTouch = Physics2D.OverlapBox(groundCheck.transform.position, floorBox, 0f, groundLayer);
        HitLogic();
    }

  

    void HitLogic()
    {
        if (Time.time - lastBounceTime < 0.1f) return; // 0.1 second cooldown

        if (rightTouch)
        {
            dirX = -Random.Range(minAngle, maxAngle);
            spriteRenderer.flipX = true;
            lastBounceTime = Time.time;
        }
        else if (leftTouch)
        {
            dirX = Random.Range(minAngle, maxAngle);
            spriteRenderer.flipX = false;
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



