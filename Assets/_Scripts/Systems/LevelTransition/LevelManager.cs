using System;
using UnityEngine;

[RequireComponent(typeof(LevelTransitioner))]
public class LevelManager : MonoBehaviour
{
    public static event Action OnLevelChange;
    
    [SerializeField] private LevelData[] levels;
    
    private LevelTransitioner _levelTransitioner;
    private int _currentLevelIndex;
    
    public LevelData CurrentLevel
    {
        get => levels[_currentLevelIndex];
        set => levels[_currentLevelIndex] = value;
    }

    public int CurrentLevelIndex
    {
        get => _currentLevelIndex;
        set
        {
            if(_currentLevelIndex == value) return;
            TransitionToLevel(value, null);
            _currentLevelIndex = value;
        }
    }
    
    public int LevelsCount => levels.Length;
    public bool DuringTransition => _levelTransitioner.DuringTransition;

    private void Start()
    {
        _levelTransitioner = GetComponent<LevelTransitioner>();
    }
    
    public void TransitionToLevel(int levelIndex, Action callback)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            return;
        }

        _currentLevelIndex = levelIndex;
        callback += OnLevelChanged;
        
        if(DialogueManager.HasInstance)
            DialogueManager.Instance.StopDialogue();
        
        if(levels[_currentLevelIndex].Music != null)
            AudioManager.Instance.PlayMusic(levels[_currentLevelIndex].Music);
        else AudioManager.Instance.StopMusic();
        
        _levelTransitioner.TransitionToLevel(levels[_currentLevelIndex].LevelScene, callback);
    }

    private void OnLevelChanged()
    {
        //If is the first time the level is loaded, we need to initialize the player position
        if (!GameStateCapturer.Instance.HasSave($"Level{_currentLevelIndex}_Restart"))
        {
            GameManager.Instance.PositionPlayer();
            GameStateCapturer.Instance.CaptureGameState($"Level{_currentLevelIndex}_Restart");
        }
        
        GameStateCapturer.Instance.CaptureGameState($"gameData");
        
        Timer.Instance.IsRunning = CurrentLevel.IsTimerActive;
        OnLevelChange?.Invoke();
    }
    
    public void TransitionToNextLevel()
    {
        if (_currentLevelIndex + 1 >= levels.Length)
        {
            Debug.LogWarning("No more levels to transition to.");
            return;
        }

        TransitionToLevel(_currentLevelIndex + 1, null);
    }
    
    public void TransitionToMenu(Action callback)
    {
        _levelTransitioner.TransitionToLevel(0, callback);
    }
    
    public void RestartCurrentLevel()
    {
        GameStateCapturer.Instance.LoadGameState($"Level{_currentLevelIndex}_Restart");
    }
}   