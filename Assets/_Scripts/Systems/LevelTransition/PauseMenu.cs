using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private OptionsMenu optionsMenu;
    
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    
    private void Awake()
    {
        GameManager.OnGamePaused += OnGamePaused;
        
        resumeButton.onClick.AddListener(GameManager.Instance.TogglePause);
        restartButton.onClick.AddListener(GameManager.Instance.Restart);
        quitButton.onClick.AddListener(GameManager.Instance.GoToMainMenu);
        optionsButton.onClick.AddListener(optionsMenu.ShowOptionsPanel);
    }

    private void Update()
    {
        if(InputManager.Instance.Escape.WasPressedThisFrame())
        {
            GameManager.Instance.TogglePause();
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGamePaused -= OnGamePaused;
        
        resumeButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        optionsButton.onClick.RemoveAllListeners();
    }
    
    private void OnGamePaused(bool isPaused)
    {
        if(!isPaused && optionsMenu.IsOptionsPanelActive) optionsMenu.HideOptionsPanel();
        pauseMenu.SetActive(isPaused);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }
}