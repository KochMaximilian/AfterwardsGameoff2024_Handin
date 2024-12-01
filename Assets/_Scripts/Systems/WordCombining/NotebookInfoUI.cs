using UnityEngine;
using UnityEngine.UI;

public class NotebookInfoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject notebookInfoPanel;
    [SerializeField] private Button openInfoButton;
    
    private bool _isPanelActive;
    public bool IsPanelActive => _isPanelActive;
    
    private void Start()
    {
        openInfoButton.onClick.AddListener(ShowInfoPanel);
        GameManager.OnGamePaused += CorrectTimeScale;
    }

    private void OnDestroy()
    {
        openInfoButton.onClick.RemoveListener(ShowInfoPanel);
        GameManager.OnGamePaused -= CorrectTimeScale;
    }

    private void ShowInfoPanel()
    {
        if(notebookInfoPanel.activeInHierarchy)
        {
            HideInfoPanel();
            return;
        }
        
        _isPanelActive = true;
        notebookInfoPanel.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void HideInfoPanel()
    {
        _isPanelActive = false;
        notebookInfoPanel.SetActive(false);
        Time.timeScale = 1;
    }
    
    private void CorrectTimeScale(bool isPaused)
    {
        if(!isPaused && _isPanelActive)
        {
            Time.timeScale = 0;
        }
    }
}