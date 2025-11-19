using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    public float destroyTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject,destroyTime);
    }
}
