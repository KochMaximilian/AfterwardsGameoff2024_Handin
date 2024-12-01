using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private GameObject containerUI;
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private TextMeshProUGUI interactTextUI;

    private IInteractable _interactable;

    private void Update()
    {
        if (playerInteractor.Interactable != null)
        {
            Show(playerInteractor.Interactable);
        }
        else
        {
            Hide();
        }
    }
    private void Show(IInteractable interactable)
    {
        if(_interactable != interactable)
        {
            containerUI.SetActive(true);
            interactable.Highlight(true);
            interactTextUI.text = interactable.GetInteractText();
        }
        
        if(_interactable != null && _interactable != interactable)
        {
            _interactable.Highlight(false);
        }
        
        _interactable = interactable;
    }

    private void Hide()
    {
        containerUI.SetActive(false);
        if (_interactable != null)
        {
            _interactable.Highlight(false);
            _interactable = null;
        }
    }
}