using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;

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
    [Header("Parry Duration&Cooldown")]

    public float parryDuration = 0.2f;
    public float parryCooldown = 0.5f;
    public float parryTickRate = 0.05f;

    public Transform firePoint;

    void Start()
    {
        ParryColor = inActiveParryColor;
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
                gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");

                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                Vector2 newDirectrion;
                Vector3 pointPost = firePoint.position;
                newDirectrion = (pointPost - projectile.transform.position).normalized;
                rb.linearVelocity = newDirectrion * 100;
           
            
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
