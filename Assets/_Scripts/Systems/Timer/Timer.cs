using System;
using UnityEngine;
using UnityUtils;

public class Timer : Singleton<Timer>
{
    public static event Action OnTimerEnd;
    public static event Action<float> OnTimerPenalized; 
    
    [SerializeField] private float initialTime;
    [SerializeField] private float timeSpeed = 1;
    [SerializeField] private bool isCountingDown = true;
    
    private float _currentTime;
    private bool _isRunning;
    private bool _initialized;

    public float CurrentTime
    {
        get
        {
            if (!_initialized)
            {
                Initialize();
            }
            return _currentTime;
        }
        set
        {
            Initialize();
            _currentTime = value;
        }
    }

    public bool IsRunning
    {
        get => _isRunning;
        set => _isRunning = value;
    }
    
    private void Start()
    {
        if (!_initialized)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        _isRunning = true;
        _currentTime = initialTime;
        _initialized = true;
    }
    
    private void Update()
    {
        if (!_isRunning || !_initialized) return;
        
        _currentTime += isCountingDown ? -Time.deltaTime * timeSpeed : Time.deltaTime * timeSpeed;
        if (_currentTime <= 0)
        {
            _currentTime = 0;
            _isRunning = false;
            GameManager.Instance.OnTimerEnd();
            OnTimerEnd?.Invoke();
        }
    }
    
    public void Penalize(float penalty)
    {
        _currentTime -= penalty;
        OnTimerPenalized?.Invoke(penalty);
    }
}