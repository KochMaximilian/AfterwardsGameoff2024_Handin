using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainManu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OptionsMenu optionsMenu;
    
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button closeCreditsButton;
    [SerializeField] private Button optionsButton;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject creditsPanel;
    
    private void Awake()
    {
        newGameButton.onClick.AddListener(GameManager.Instance.NewGame);
        loadGameButton.onClick.AddListener(GameManager.Instance.LoadGame);
        quitButton.onClick.AddListener(GameManager.Instance.ExitGame);
        creditsButton.onClick.AddListener(ShowCredits);
        closeCreditsButton.onClick.AddListener(CloseCredits);
        optionsButton.onClick.AddListener(optionsMenu.ShowOptionsPanel);
    }

    private void OnDestroy()
    {
        newGameButton.onClick.RemoveAllListeners();
        loadGameButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        creditsButton.onClick.RemoveAllListeners();
        closeCreditsButton.onClick.RemoveAllListeners();
        optionsButton.onClick.RemoveAllListeners();
    }
    
    private void ShowCredits()
    {
        creditsPanel.SetActive(true);
        creditsPanel.transform.localScale = Vector3.zero;
        creditsPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    
    private void CloseCredits()
    {
        creditsPanel.transform.localScale = Vector3.one;
        creditsPanel.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => creditsPanel.SetActive(false));
    }
}