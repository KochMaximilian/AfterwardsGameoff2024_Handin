using System.Collections.Generic;
using UnityEngine;

public class RadialGetInteractableStrategy : IGetInteractableStrategy
{
    private readonly Transform _transform;
    private readonly float _interactionRange;
    private readonly LayerMask _interactableLayer;
    
    public RadialGetInteractableStrategy(Transform transform, float interactionRange, LayerMask interactableLayer)
    {
        _transform = transform;
        _interactionRange = interactionRange;
        _interactableLayer = interactableLayer;
    }

    public IInteractable Execute()
    {
        List<IInteractable> interactableObjects = new List<IInteractable>();
        
        Collider[] colliders = Physics.OverlapSphere(_transform.position, _interactionRange, _interactableLayer);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out IInteractable npcInteractable))
            {
                interactableObjects.Add(npcInteractable);
            }
        }
        
        IInteractable closestObject = null; //Interact with closest interactableObject
        foreach (IInteractable interactableObject in interactableObjects)
        {
            if (closestObject == null)
            {
                closestObject = interactableObject;
            }
            else
            {
                if (Vector3.Distance(_transform.position, interactableObject.GetTransform().position) <
                    Vector3.Distance(_transform.position, closestObject.GetTransform().position))
                {
                    closestObject = interactableObject;
                }
            }
        }
        
        return closestObject;
    }
}