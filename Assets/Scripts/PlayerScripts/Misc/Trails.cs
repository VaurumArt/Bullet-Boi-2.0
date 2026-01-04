using UnityEngine;

public class Trails : MonoBehaviour
{
    [HeaderAttribute("Trails")]
    public GameObject fast;
    public GameObject rapid;
    public GameObject blazing;
    public GameObject sonic;
    public GameObject lightspeed;

    public float fastSpeed=30;
    public float rapidSpeed=60;
    public float blazingSpeed=90;
    public float sonicSpeed=120f;
    public float lightSpeed=150;

    private GameObject currentActiveTrail;
    private float playerSpeed;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trailDisabler();
    }

    // Update is called once per frame
    void Update()
    {

        playerSpeed = rb.linearVelocity.magnitude;
        TrailChecker();
    }

    void trailDisabler()
    {
        fast.SetActive(false);
        rapid.SetActive(false);
        blazing.SetActive(false);
        sonic.SetActive(false);
        lightspeed.SetActive(false);

    }
    void TrailChecker()
    {
        GameObject trailTarget = null;

        if (playerSpeed >= lightSpeed)
        {
            trailTarget = lightspeed;
        }
        else if (playerSpeed >= sonicSpeed)
        {
            trailTarget = sonic;
        }
        else if (playerSpeed >= blazingSpeed)
        {
            trailTarget = blazing;
        }
        else if (playerSpeed >= rapidSpeed)
        {
            trailTarget = rapid;
        }
        else if (playerSpeed >= fastSpeed)
        {

            trailTarget = fast;
        }

        if (trailTarget != currentActiveTrail)
        {
            if (currentActiveTrail != null)
            {
                currentActiveTrail.SetActive(false);
            }

            if (trailTarget != null)
            {
                trailTarget.SetActive(true);
            }
            currentActiveTrail = trailTarget;
        }
    }
}
