using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CountdownTimer : MonoBehaviour
{
    public int startTimeInSeconds = 25;
    public TextMeshProUGUI countdownText;
    public UnityEvent onTimeUp;
    private float currentTime;

    void Start()
    {
        currentTime = startTimeInSeconds;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            countdownText.text = Mathf.Ceil(currentTime).ToString();

            if (currentTime < 7)
            {
                countdownText.color = Color.red;
            }
            else if (currentTime < 15)
            {
                countdownText.color = new Color(1.0f, 0.65f, 0.0f);
            }
            else
            {
                countdownText.color = Color.white;
            }
        }
        else
        {
            countdownText.text = "Time's up!";
            onTimeUp?.Invoke();
            enabled = false;
        }
    }

    public void StopCountdown()
    {
        currentTime = 0; 
        enabled = false; 
    }
}
