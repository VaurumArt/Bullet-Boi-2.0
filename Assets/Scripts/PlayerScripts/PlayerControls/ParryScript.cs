using System.Collections;
using UnityEngine;

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
            Collider2D [] parriedProjectile  = Physics2D.OverlapCircleAll(parryCenter.position, range,projectileLayers);//Hurt box to parry projectiles 

            foreach (Collider2D projectile in parriedProjectile )// Check each projectile that was parried 
            {
                Debug.Log("Parried " + projectile.name);
            }
        }
        else if (!canParry)
        {
            Debug.Log("Parry is on cooldown");
        }
    }
    IEnumerator ParryingActivated()
    {
        isParrying = true;
        ParryColor = activeParryColor;
        yield return new WaitForSeconds(parryDuration);
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
