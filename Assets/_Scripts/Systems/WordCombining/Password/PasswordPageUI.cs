using UnityEngine;
using UnityEngine.UI;

public class PasswordPageUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    
    [Header("References")]
    [SerializeField] private PasswordNavigationManager passwordNavigationManager;

    private void Awake()
    {
        PasswordCollection.OnInitialized += Initialize;
    }
    
    private void OnDestroy()
    {
        PasswordCollection.OnInitialized -= Initialize;
    }
    
    private void Start()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);
        prevButton.onClick.AddListener(OnPrevButtonClicked);
    }

    private void Initialize()
    {
        if(passwordNavigationManager.PasswordCollection.Passwords.Count <= 1)
        {
            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);
        }
    }

    private void OnNextButtonClicked()
    {
        nextButton.OnDeselect(null);
        passwordNavigationManager.NavigateToNextPassword();
        WordSounds.PlayNavigateThroughPasswordsSound();
        
        DisableButtons();
    }
    
    private void OnPrevButtonClicked()
    {
        nextButton.OnDeselect(null);
        passwordNavigationManager.NavigateToPrevPassword();
        WordSounds.PlayNavigateThroughPasswordsSound();

        DisableButtons();
    }
    
    private void EnableButtons()
    {
        passwordNavigationManager.OnAnimationFinished -= EnableButtons;
        nextButton.interactable = true;
        prevButton.interactable = true;
    }
 
    private void DisableButtons()
    {
        nextButton.interactable = false;
        prevButton.interactable = false;
        passwordNavigationManager.OnAnimationFinished += EnableButtons;
    }
}