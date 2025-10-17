using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlowMoScript : MonoBehaviour
{
    [Header("Focus Time Slow")]
    public float slowMoFactor = 0.75f;
    [Header("Focus Bar")]
    public float maxFocus = 50f;
    public float focusBar = 50f;
    [Header("Focus Drain&Refill")]
    public float focusConsumeTime = 30f;
    public float focusReFillTime = 10f;
    private bool isSlowMo = false;
    private bool isTimeStop = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        focusBar = maxFocus;  
    }

    // Update is called once per frame
    void Update()
    { 
        focusBar = Mathf.Clamp(focusBar,0, maxFocus);
        FocusConsume();
    }
    void FocusConsume()
    {
        if (!isTimeStop && isSlowMo && focusBar > 0)
        {
            focusBar -= focusConsumeTime * Time.unscaledDeltaTime;
            if (focusBar <= 0)
            {
                SlowMoReleased();
            }
            //   StartCoroutine(ConsumeFocus());
        }
        else if (!isTimeStop && !isSlowMo && focusBar < 100)
        {
            focusBar += focusReFillTime * Time.deltaTime;
            //  StartCoroutine(RefillFocus());

        }
    }

   public void TimeStopper ()
    {
        isTimeStop = true;
    }
   public  void TimeResume()
    {
        isTimeStop = false;
    }

    public void OnSlowMo(InputAction.CallbackContext context)
    {
        if (context.started )
        {
            isSlowMo = true;
            Debug.Log("SlowMo Mode Activated");
            if (focusBar >= 0 && isSlowMo)
            {
                TimeSlowing();
            }

        }
        else if (context.canceled)
        { 
            isSlowMo = false;
            Debug.Log("SlowMO Mode Canceled");
            SlowMoReleased();
        }
    }

    public void SlowMoReleased()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
   public void TimeSlowing()
    {
 
    Time.timeScale = slowMoFactor;
    Time.fixedDeltaTime = Time.timeScale * .02f;

    }
    
}
