using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private float elapsedTime;
    private bool isRunning;

    public float ElapsedTime => elapsedTime;

    private void Start()
    {
        StartTimer();
    }

    private void Update()
    {
        if (!isRunning)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        UpdateTimerText();
    }

    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;

        UpdateTimerText();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int totalSeconds = Mathf.FloorToInt(elapsedTime);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }


}