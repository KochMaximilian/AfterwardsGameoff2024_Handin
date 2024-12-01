using UnityEngine;
using UnityUtils;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private LayerMask interactableLayer;
    
    private IGetInteractableStrategy _strategy;
    private IInteractable _interactable;
    
    public IInteractable Interactable => _interactable;
    
    private void Start()
    {
        //_strategy = new RadialGetInteractableStrategy(transform, interactionRange, interactableLayer);
        _strategy = new RayGetInteractableStrategy(Helpers.Camera.transform, interactionRange, interactableLayer);
    }
    
    private void Update()
    {
        _interactable = GetInteractableObject();
        
        if (InputManager.Instance.Interact.WasPressedThisFrame())
        {
            if (_interactable != null)
            {
                _interactable.Interact();
            }
        }
    }

    private IInteractable GetInteractableObject()
    {
        return _strategy.Execute();
    }
}