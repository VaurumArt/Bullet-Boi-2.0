using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.UI;
using UnityEditor.Build;
using TMPro;
public class ScoreSystemScript : MonoBehaviour
{
    [SerializeField]  PlayerMovement playerMovement;
 
    public float score;
    [SerializeField] public TextMeshProUGUI scoreText;
    public float combo;
    public TextMeshProUGUI comboText;
    public float mult;
    public TextMeshProUGUI multText;
    public float comboWindow = 1f;
    public float comboTimer;
    public float airborneTimer;

    private bool isTeleported = false;
    
    private void Start()
    {
    
    }
    private void Update()
    {
        mult = 1 + (combo * 0.1f);
        scoreText.text = score.ToString();
        multText.text = mult.ToString() + "X";

        ComboDisplay();
        AirborneCheck();
        ComboStacker();

    }
    private void ComboDisplay()
    {
        if (combo > 0)
        {
            comboText.text = combo.ToString();
        }
        else
        {
            comboText.text = "";
        }
    }
    private void ComboStacker()
    {
        if (combo > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer >= comboWindow)
            {
                combo = 0;
                comboTimer = 0;
            }
        }
    }

    private float GetMultiplier()
    {
     
        return 1 + (combo * 0.1f);
    }
    public void KillRegister()
    {
        if (isTeleported)
        {
            TeleportKillScore();
        }
        else
        {
            RamKillScore();
        }
    }
    //KILLS score
    public void RamKillScore()
    {

        score += 100 * GetMultiplier();
        combo++; 
        comboTimer = 0;
    }
    public void BulletKillScore()
    {
        score += 100 * GetMultiplier();
        combo++; 
        comboTimer = 0;
    }

    public void BiteKillScore()
    {
        score += 200 * GetMultiplier();
        combo++;
        comboTimer = 0;
    }

    public void ParryKillScore()
    {
        score += 200 * GetMultiplier();
        combo++;
        comboTimer = 0;
    }

    public void TeleportCheck()
    {
        StartCoroutine(TeleportKillWait());
    }
    public void TeleportKillScore()
    {//work on this 


        score += 100 * GetMultiplier();
        combo++; 
        comboTimer = 0;

    }

    IEnumerator TeleportKillWait()
    {
        isTeleported = true;
        yield return new WaitForSeconds(0.75f);
        isTeleported = false;
    }

    //MOVEMENT score


    public void SuccessfullParryScore()
    {
        score += 200 * GetMultiplier();
        combo++;
        comboTimer = 0;
    }

    public void PerfectWallBounceScore()
    {
        score += 300 * GetMultiplier();
        combo++;
        comboTimer = 0;
    }
    public void GoodWallBounceScore()
    {
        score += 100 * GetMultiplier();
        combo++;
        comboTimer = 0;
    }
    // INCREMENTAL score

    public void AirborneCheck()
    {

        if (!playerMovement.isGrounded)
        {
            airborneTimer += Time.deltaTime;

            if (airborneTimer >= 5) 
            {
                score += 100 * GetMultiplier();
                airborneTimer -= 5;
            }
        }
        else
        {
            airborneTimer = 0;
        }

    }

    public void SpeedMaintainScore()
    {

    }
    public void SpeedUpScore()
    {

    }
}
