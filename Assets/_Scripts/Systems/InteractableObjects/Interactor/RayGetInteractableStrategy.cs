using UnityEngine;

public class RayGetInteractableStrategy : IGetInteractableStrategy
{
    private readonly Transform _transform;
    private readonly float _interactionRange;
    private readonly LayerMask _interactableLayer;

    public RayGetInteractableStrategy(Transform transform, float interactionRange, LayerMask interactableLayer)
    {
        _transform = transform;
        _interactionRange = interactionRange;
        _interactableLayer = interactableLayer;
    }

    public IInteractable Execute()
    {
        if (!Physics.Raycast(_transform.position, _transform.forward, out var hit, _interactionRange, _interactableLayer)) return null;
        if (hit.collider.TryGetComponent(out IInteractable interactable))
        {
            if(!interactable.CanInteract()) return null;
            return interactable;
        }

        return null;
    }
}