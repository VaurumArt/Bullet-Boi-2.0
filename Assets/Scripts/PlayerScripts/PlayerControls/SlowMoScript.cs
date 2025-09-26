using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlowMoScript : MonoBehaviour
{

    public float slowMoFactor = 0.75f;
    public bool isSlowMo = false;
    public float focusBar = 50f;
    public float maxFocus = 50f;

    public float focusConsumeTime = .5f;
    public float focusReFillTime = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        focusBar = maxFocus;
        
    }

    // Update is called once per frame
    void Update()
    { 
        focusBar = Mathf.Clamp(focusBar,0, maxFocus);
        if (isSlowMo && focusBar > 0)
        {
            focusBar -= 30 * Time.unscaledDeltaTime;// fix this 

            if (focusBar <= 0)
            {
                SlowMoReleased();
            }
         //   StartCoroutine(ConsumeFocus());
        }
        else if (!isSlowMo && focusBar < 100)
        {
            focusBar += 10 * Time.unscaledDeltaTime;
          //  StartCoroutine(RefillFocus());
        
        }
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
