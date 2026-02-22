using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameTimer : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI timerText;
    float elapsedTime;

    // Update is called once per frame
    void Update()
    {
      elapsedTime += Time.unscaledDeltaTime;  
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds =Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}",minutes,seconds);
    }
}
