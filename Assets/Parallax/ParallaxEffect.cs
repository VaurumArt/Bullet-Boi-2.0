using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float dinstance = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startPos + dinstance, transform.position.y, transform.position.z);
    }
    void Update()
    {
       
    }
}
