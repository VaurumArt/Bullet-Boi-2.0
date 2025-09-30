using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

public class ParryScript : MonoBehaviour
{
    [Header("Parry HurtBox")]

    public Transform parryCenter;
    public float range = 3f;
    public LayerMask projectileLayers;

    [Header("Parry Checker")]

    public bool isParrying = false;
    public bool canParry = true;
    public Color ParryColor;
    public Color activeParryColor = Color.purple;
    public Color inActiveParryColor = Color.blue;
    public Color CooldownParryColor = Color.yellow;
    [Header("Parry Duration & CD")]
    
    public float parryDuration = 0.2f;
    public float parryCooldown = 0.5f;
    public float parryTickRate = 0.05f;
    public float parrySpeedBoost = 300;


    void Start()
    {
        ParryColor = inActiveParryColor;
    }
    private void Update()
    {
     

    }
    public void OnParry()
    {
        if (!isParrying && canParry)
        {
            Debug.Log("Parrying!");
            StartCoroutine(ParryingActivated());
        }
        else if (!canParry)
        {
            Debug.Log("Parry is on cooldown");
        }
    }
    void PerformParry()
    {
    
        Collider2D[] parriedProjectile = Physics2D.OverlapCircleAll(parryCenter.position, range, projectileLayers);//Hurt box to parry projectiles 

        foreach (Collider2D projectile in parriedProjectile)// Check each projectile that was parried 
        {
       
            Debug.Log("Parried " + projectile.name);
            EnemyBullet enemyBullet = projectile.GetComponent<EnemyBullet>();
            if(enemyBullet != null)
            {
                Rigidbody2D rbBullet = projectile.GetComponent<Rigidbody2D>();

                Vector2 newDirectrion;
                Vector2 screenMousePos = Mouse.current.position.ReadValue();
                // Convert to world space (for 2D orthographic camera)
                Vector3 worldMousePos3D = Camera.main.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, Camera.main.nearClipPlane));
                Vector2 mouseWorldPos2D = (Vector2)worldMousePos3D;
                // Calculate normalized direction from projectile to mouse
                Vector2 newDirection = (mouseWorldPos2D - (Vector2)projectile.transform.position).normalized;

              

                rbBullet.linearVelocity = newDirection * parrySpeedBoost;

                rbBullet.gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");

                rbBullet.gameObject.tag = "Bullet";
            
            }
           
                
        }

    }
    IEnumerator ParryingActivated()
    {
        isParrying = true;
        ParryColor = activeParryColor;
        float ElapsedTime = 0;
        while (ElapsedTime < parryDuration)
        {
            PerformParry();

            yield return new WaitForSeconds(parryTickRate);
            ElapsedTime += parryTickRate;
        }

        isParrying = false;
        ParryColor = Color.blue;
        StartCoroutine(ParryingisCoolingdown());
       
    }
    IEnumerator ParryingisCoolingdown()
    {
        canParry = false;
        ParryColor = CooldownParryColor;
        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
        ParryColor = inActiveParryColor;
        Debug.Log("Parry is Available");
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = ParryColor;
        Gizmos.DrawWireSphere(parryCenter.position, range);
    }
}
