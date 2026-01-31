using UnityEngine;
using UnityEngine.UIElements;

public class ObjectInfo : MonoBehaviour
{
   public  Rigidbody2D rbObject;
    public float parrySpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rbObject = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
