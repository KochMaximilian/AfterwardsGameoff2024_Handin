using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityUtils;

[RequireComponent(typeof(LevelManager))]
public class GameManager : PersistentSingleton<GameManager>
{
    public static event Action<bool> OnGamePaused;
    public static event Action OnGameOver;
    public static event Action OnGoToMainMenu; 
    
    private LevelManager _levelManager;
    
    private bool _isPaused;
    private bool _isGameOver;
    
    private CursorLockMode _previousLockMode;
    private bool _previousCursorVisibility;
    
    public bool CanUseNotebook { get; set; } = true;
    public LevelManager LevelManager => _levelManager;
    public bool IsPaused => _isPaused;
    public bool IsGameOver => _isGameOver;
    public bool HasFirstInteractedWithNotebook { get; set; }

    protected override void Awake()
    {
        base.Awake();
        _levelManager = GetComponent<LevelManager>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            InitializeMenu();
        }
    }

    public void NewGame()
    {
        DeleteAllData();
        
        LevelManager.CurrentLevelIndex = -1;
        _isGameOver = false;
        LevelManager.TransitionToLevel(0, null);
    }

    private void DeleteAllData()
    {
        for (int i = 0; i <  LevelManager.LevelsCount; i++)
        {
            GameStateCapturer.Instance.DeleteGameState($"Level{i}_Restart");
        }

        GameStateCapturer.Instance.DeleteGameState("gameData");
        SentenceProcessor.ClearCache();
    }

    public void LoadGame()
    {
        LevelManager.CurrentLevelIndex = -1;

        if (!GameStateCapturer.Instance.LoadGameState("gameData"))
        {
            NewGame();
        }
        
        _isGameOver = false;
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        Timer.Instance.IsRunning = false;
        ResumeGame();
        LevelManager.RestartCurrentLevel();
        
        _isGameOver = false;
    }
    
    public void NextLevel()
    {
        LevelManager.TransitionToNextLevel();
    }
    
    public void GoToLevel(int levelIndex)
    {
        LevelManager.CurrentLevelIndex = levelIndex;
    }
    
    public void GoToMainMenu()
    {
        ResumeGame();
        _isGameOver = false;

        GameStateCapturer.Instance.CaptureGameState("gameData");
        LevelManager.TransitionToMenu(InitializeMenu);
    }

    private void PauseGame()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0) return;
        
        _previousCursorVisibility = Cursor.visible;
        _previousLockMode = Cursor.lockState;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        _isPaused = true;
        Time.timeScale = 0;
        OnGamePaused?.Invoke(true);
    }

    private void ResumeGame()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0) return;

        Cursor.lockState = _previousLockMode;
        Cursor.visible = _previousCursorVisibility;
        
        _isPaused = false;
        Time.timeScale = 1;
        OnGamePaused?.Invoke(false);
    }

    public void TogglePause()
    {
        if(_isGameOver || LevelManager.DuringTransition) return;
        
        if (_isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PositionPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");
        if(playerSpawn != null && player != null)
        {
            player.transform.position = playerSpawn.transform.position;
            CameraController.Instance.SetRotation(playerSpawn.transform.rotation.eulerAngles);
        }
    }
    
    private void InitializeMenu()
    {
        GameplayItems.Instance.ClearItems();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Timer.Instance.IsRunning = false;
        CoroutineController.StopAll();
        LevelManager.CurrentLevelIndex = -1;
        AudioManager.Instance.PlayMusic(AudioManager.Instance.Sfx.MainMenuMusic);
    }

    public void OnTimerEnd()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0) return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isGameOver = true;
        
        OnGameOver?.Invoke();
    }
}