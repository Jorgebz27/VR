using System;
using UnityEngine;
using TMPro;
using static UnityEngine.SceneManagement.SceneManager;

public class TimerController : MonoBehaviour
{
    private float elapsedTime = 0f;
    private bool timerActive = false;
    public TextMeshProUGUI timerText;

    public void StartTimer()
    {
        elapsedTime = 0f;
        timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    private void Update()
    {
        if (timerActive)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            float seconds = elapsedTime % 60f;
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
