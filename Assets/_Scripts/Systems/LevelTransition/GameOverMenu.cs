using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverMenu;
    
    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;
    
    private void Awake()
    {
        GameManager.OnGameOver += OnGameOver;
        LevelManager.OnLevelChange += OnGameRestart;
        
        restartButton.onClick.AddListener(GameManager.Instance.Restart);
        quitButton.onClick.AddListener(GameManager.Instance.GoToMainMenu);
    }

    private void OnDestroy()
    {
        GameManager.OnGameOver -= OnGameOver;
        LevelManager.OnLevelChange -= OnGameRestart;
        
        restartButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }
    
    private void OnGameOver()
    {
        gameOverMenu.SetActive(true);
    }
    
    private void OnGameRestart()
    {
        gameOverMenu.SetActive(false);
    }
}