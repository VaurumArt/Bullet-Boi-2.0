using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
public class ParryScript : MonoBehaviour
{
    [SerializeField] ScoreSystemScript ScoreScript;
    public float kickDamage = 5f;
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
    public LayerMask enemyLayers;
    public LayerMask objectLayers;

    [Header("Parry Duration & CD")]
    public float parryDuration = 0.2f;
    public float parryCooldown = 0.5f;
    public float parryTickRate = 0.05f;
    public float parrySpeedBoost = 300;

    [Header("Parry  Slow Mo")]
    public float parryPauseFactor = 1f;
    public float parrypausetime = 1f;
    private SlowMoScript slowMoScript;
    void Start()
    {
        slowMoScript = GetComponent<SlowMoScript>();
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
            EnemyInfo enemyInfo = enemy.GetComponent<EnemyInfo>(); // Changed to enemyInfo
            if (enemyInfo != null) // Check if enemyInfo exists, not enemyBody
            {
                ParryEnemy(enemy, enemyInfo); // Pass enemy and enemyInfo, not enemyBullet
           
            }
        }

        Collider2D[] parriedObjects = Physics2D.OverlapBoxAll(offsetPosition, parrySize, attackPoint.eulerAngles.z, objectLayers);
        foreach (Collider2D obj in parriedObjects) // Changed 'object' to 'obj' - 'object' is a reserved keyword!
        {
            ObjectInfo objectInfo = obj.GetComponent<ObjectInfo>(); // Changed EnemyInfo to ObjectInfo
            if (objectInfo != null)
            {
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

        rbBullet.linearVelocity = newDirection * (rbBullet.linearVelocity.magnitude * parrySpeedBoost);
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
    
        objectInfo.rbObject.linearVelocity = newDirection * objectInfo.parrySpeed;
        
        StartCoroutine(parryPauseTime());
        ScoreScript.SuccessfullParryScore();
    }
    void ParryEnemy(Collider2D enemy, EnemyInfo enemyInfo)
    {
        // Use the rbEnemy reference that's already in EnemyInfo
        if (enemyInfo.rbEnemy == null) return;

        EnemyHP enemyHP = enemy.GetComponent<EnemyHP>();

        Vector2 screenMousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos3D = Camera.main.ScreenToWorldPoint(new Vector3(screenMousePos.x, screenMousePos.y, Camera.main.nearClipPlane));
        Vector2 mouseWorldPos2D = (Vector2)worldMousePos3D;
        Vector2 newDirection = (mouseWorldPos2D - (Vector2)enemy.transform.position).normalized;
        enemyHP.TakeDamage(kickDamage);
        // Launch the enemy toward the mouse with the parry speed
        enemyInfo.rbEnemy.linearVelocity = newDirection * enemyInfo.parrySpeed;
        enemyInfo.GetParried();
        StartCoroutine(parryPauseTime());
        ScoreScript.SuccessfullParryScore();
    }
    IEnumerator ParryingActivated()
    {
        isParrying = true;
        parryColor = activeParryColor;
        float ElapsedTime = 0;
        parryFeedback?.PlayFeedbacks();
        while (ElapsedTime < parryDuration)
        {
            PerformParry();
            yield return new WaitForSeconds(parryTickRate);
            ElapsedTime += parryTickRate;
        }

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