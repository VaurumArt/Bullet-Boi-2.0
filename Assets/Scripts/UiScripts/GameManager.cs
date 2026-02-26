using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int enemyCounter=0; 
    PlayerHP playerHP;

    public GameObject winCon;
    public GameObject loseCon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHP = FindAnyObjectByType<PlayerHP>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WinCondition()
    {
        if(enemyCounter == 0)
        {
            Debug.Log("You Win");
            winCon.SetActive(true);
        }
    }
    public void LoseCondition()
    {
        if (!playerHP.isAlive)
        {
            Debug.Log("You Lose");
            loseCon.SetActive(true);
        }
    }

    public void EnemyAlive()
    {
        
        enemyCounter +=1;
        Debug.Log(enemyCounter);

    }
    public void EnemyDeath()
    {

        enemyCounter -= 1;
        Debug.Log(enemyCounter);
        WinCondition();

    }

}
