using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
public class ParryScript : MonoBehaviour
{
    [SerializeField] ScoreSystemScript ScoreScript;
    public float kickDamage = 5f;
    public float parryMultiplier = 1f;  
    public MMFeedbacks parryFeedback;
    [Header("Parry Checker")]
    private bool isParrying = false;
    private bool canParry = true;
    private Color parryColor;
    public Color activeParryColor = Color.green;
    public Color inActiveParryColor = Color.white;
    public Color CooldownParryColor = Color.yellow;

    [Header("Parry Detection")]
    public Transform attackPoint; // Changed from parryCenter to match BiteScript
    public Vector2 parrySize = new Vector2(2, 2); // Same as biteSize

    public LayerMask projectileLayers;
    public float projectileParrySpeed = 1;
    public LayerMask enemyLayers;
    public float enemyParrySpeed = 1;
    public LayerMask objectLayers;
    public float objectParrySpeed = 1;

    [Header("Parry Duration & CD")]
    public float parryDuration = 0.2f;
    public float parryCooldown = 0.5f;
    public float parryTickRate = 0.05f;
    public float parrySpeedBoost = 300;

    [Header("Parry  Slow Mo")]
    public float parryPauseFactor = 1f;
    public float parrypausetime = 1f;
    private SlowMoScript slowMoScript;
    private AnimationHandler animationHandler;
    private HashSet<Collider2D> hitObjectThisParry = new HashSet<Collider2D>();
    void Start()
    {
        slowMoScript = GetComponent<SlowMoScript>();
        animationHandler = GetComponent<AnimationHandler>();
        parryColor = inActiveParryColor;
    }
    IEnumerator parryPauseTime()
    {
        slowMoScript.TimeStopper();
        Time.timeScale = parryPauseFactor;
        Time.fixedDeltaTime = Time.timeScale * .02f;
        yield return new WaitForSecondsRealtime(parrypausetime);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        slowMoScript.TimeResume();
    }
    IEnumerator ParryingActivated()
    {

        isParrying = true;
        parryColor = activeParryColor;
        float ElapsedTime = 0;
        parryFeedback?.PlayFeedbacks();
        animationHandler.ParryAnimationOn();
        hitObjectThisParry.Clear();
        while (ElapsedTime < parryDuration)
        {
            PerformParry();
            yield return new WaitForSeconds(parryTickRate);
            ElapsedTime += parryTickRate;
        }
        animationHandler.ParryAnimationOff();
        isParrying = false;
        StartCoroutine(ParryingisCoolingdown());

    }

    IEnumerator ParryingisCoolingdown()
    {
        canParry = false;
        parryColor = CooldownParryColor;
        yield return new WaitForSeconds(parryCooldown);
        canParry = true;
        parryColor = inActiveParryColor;
    }

    public void OnParry()
    {
        if (!isParrying && canParry)
        {
            StartCoroutine(ParryingActivated());
        }
    }

    void PerformParry()
    {

        // EXACTLY the same as BiteScript's PerformBite hitbox calculation
        Vector3 offsetPosition = attackPoint.position + attackPoint.right * (parrySize.x * 0.5f);

        // Check for projectiles
        Collider2D[] parriedProjectiles = Physics2D.OverlapBoxAll(offsetPosition, parrySize, attackPoint.eulerAngles.z, projectileLayers);
        foreach (Collider2D projectile in parriedProjectiles)
        {
           

            EnemyBullet enemyBullet = projectile.GetComponent<EnemyBullet>();
            if (enemyBullet != null)
            {
                ParryProjectile(projectile, enemyBullet);
           
            }
        }

        // Check for enemies
        Collider2D[] parriedEnemies = Physics2D.OverlapBoxAll(offsetPosition, parrySize, attackPoint.eulerAngles.z, enemyLayers);
        foreach (Collider2D enemy in parriedEnemies) // Changed 'projectile' to 'enemy' for clarity
        {
            EnemyParryBehaviour enemyParryBehaviour = enemy.GetComponent<EnemyParryBehaviour>(); // Changed to enemyInfo
            if (enemyParryBehaviour != null) // Check if enemyInfo exists, not enemyBody
            {
                ParryEnemy(enemy, enemyParryBehaviour); // Pass enemy and enemyInfo, not enemyBullet
           
            }
        }

        Collider2D[] parriedObjects = Physics2D.OverlapBoxAll(offsetPosition, parrySize, attackPoint.eulerAngles.z, objectLayers);
        foreach (Collider2D obj in parriedObjects) // Changed 'object' to 'obj' - 'object' is a reserved keyword!
        {
            if (hitObjectThisParry.Contains(obj))
                continue;

            ObjectInfo objectInfo = obj.GetComponent<ObjectInfo>(); // Changed EnemyInfo to ObjectInfo
            if (objectInfo != null)
            {
                hitObjectThisParry.Add(obj);
                ParryObject(obj, objectInfo); // Create a separate method for objects
            }
        }
    }
    void ParryProjectile(Collider2D projectile, EnemyBullet enemyBullet)
    {
        Rigidbody2D rbBullet = projectile.GetComponent<Rigidbody2D>();
        Vector2 screenMousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos3D = Camera.main.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, Camera.main.nearClipPlane));
        Vector2 mouseWorldPos2D = (Vector2)worldMousePos3D;
        Vector2 newDirection = (mouseWorldPos2D - (Vector2)projectile.transform.position).normalized;

        rbBullet.linearVelocity = newDirection * (rbBullet.linearVelocity.magnitude * projectileParrySpeed);
        rbBullet.gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        rbBullet.gameObject.tag = "Bullet";
        StartCoroutine(parryPauseTime());
        ScoreScript.SuccessfullParryScore();
    }
    void ParryObject(Collider2D obj, ObjectInfo objectInfo)
    {
      
            // Use the rbEnemy reference that's already in EnemyInfo
            if (objectInfo.rbObject == null) return;


        
        Vector2 screenMousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos3D = Camera.main.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, Camera.main.nearClipPlane));
        Vector2 mouseWorldPos2D = (Vector2)worldMousePos3D;
        Vector2 newDirection = (mouseWorldPos2D - (Vector2)obj.transform.position).normalized;

        objectInfo.rbObject.linearVelocity = newDirection * objectInfo.parrySpeed *objectParrySpeed;
        
        StartCoroutine(parryPauseTime());
        ScoreScript.SuccessfullParryScore();
    }
    void ParryEnemy(Collider2D enemy, EnemyParryBehaviour enemyParryBehaviour)
    {
        if (!enemyParryBehaviour.isParried)
        {
            // Check if rbEnemy exists
            if (enemyParryBehaviour.rbEnemy == null) return;

            // 1. DISABLE MOVEMENT FIRST
            enemyParryBehaviour.GetParried();

            // 2. Get enemy HP
            EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();

            // 3. Calculate mouse direction
            Vector2 screenMousePos = Mouse.current.position.ReadValue();
            Vector3 worldMousePos3D = Camera.main.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, Camera.main.nearClipPlane));
            Vector2 mouseWorldPos2D = (Vector2)worldMousePos3D;
            Vector2 newDirection = (mouseWorldPos2D - (Vector2)enemy.transform.position).normalized;

            // 4. Deal damage
            if (enemyHP != null)
            {
                enemyHP.TakeDamage(kickDamage);
            }

            // 5. SET VELOCITY (now that movement is disabled)
            enemyParryBehaviour.rbEnemy.linearVelocity = newDirection * (enemyParryBehaviour.parrySpeed* enemyParrySpeed);

            // 6. Effects
            StartCoroutine(parryPauseTime());
            ScoreScript.SuccessfullParryScore();
        }

    }
   
    private void OnDrawGizmos()
    {
        // EXACTLY the same as BiteScript's OnDrawGizmos
        if (attackPoint == null) return;

        Gizmos.color = parryColor;
        Vector2 forwardOffset = new Vector2(parrySize.x * 0.5f, 0);
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(
            attackPoint.position + (Vector3)(Quaternion.Euler(0, 0, attackPoint.eulerAngles.z) * forwardOffset),
            Quaternion.Euler(0, 0, attackPoint.eulerAngles.z),
            Vector3.one
        );
        Gizmos.DrawWireCube(Vector3.zero, parrySize);
        Gizmos.matrix = oldMatrix;
    }
}