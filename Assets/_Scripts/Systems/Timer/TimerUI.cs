using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private Timer timer;
    [SerializeField] private TMP_Text timerText;
    
    private int _minutes;
    private int _seconds;

    private void Awake()
    {
        Timer.OnTimerPenalized += OnTimerPenalized;
    }

    private void OnDestroy()
    {
        Timer.OnTimerPenalized -= OnTimerPenalized;
    }

    private void Update()
    {
        if (!Timer.Instance.IsRunning)
        {
            timerText.text = "";
            timerText.color = Color.white;
            return;
        }
        
        UpdateTime();
    }

    private void UpdateTime()
    {
        if (!timer.IsRunning)
        {
            timerText.text = "";
            return;
        }
        
        _minutes = (int)timer.CurrentTime / 60;
        _seconds = (int)timer.CurrentTime % 60;
        timerText.text = $"{_minutes:00}:{_seconds:00}";
    }
    
    private void OnTimerPenalized(float timePenalized)
    {
        UpdateTime();
        transform.DOComplete(true);
        
        Vector3 initialPosition = timerText.transform.position;

        transform.DOShakePosition(0.25f, 10, 100).OnComplete(() => timerText.transform.position = initialPosition);
        timerText.DOColor(Color.red, 0.25f).OnComplete(() =>
        {
            timerText.DOColor(Color.white, 0.25f);
        });
    }
}