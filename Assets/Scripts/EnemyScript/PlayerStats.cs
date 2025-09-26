using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerStats : MonoBehaviour
{

    
    [Header("Player Speed")]
    public int playerCurrentSpeed;
    public TextMeshProUGUI speedText;
    [Header("Player Health")]
    public float health ;
    public Slider healthSlider;
    [Header("Player Mana")]
    public float focus;
    public Slider focusSlider;
    [Header("Bullet Upgrades")]


    PlayerMovement PlayerMovement;
    SlowMoScript SlowMoScript;
    PlayerHP PlayerHP;
    void Start()
    {
        SlowMoScript = GetComponent<SlowMoScript>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerHP = GetComponent<PlayerHP>();
    }

    // Update is called once per frame
    void Update()
    {
        playerCurrentSpeed = Mathf.RoundToInt(PlayerMovement.playerSpeed);

        health = PlayerHP.health;
        healthSlider.value = health; 

        focus = SlowMoScript.focusBar;
        focusSlider.value = focus;

      
        speedText.text = playerCurrentSpeed.ToString(); 
    }
}
