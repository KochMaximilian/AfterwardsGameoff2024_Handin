using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    [Header("Interactable Settings")]
    [SerializeField] protected string interactText;
    [SerializeField] protected UnityEvent onInteract;
    [SerializeField] protected UnityEvent onFirstInteract;
    [SerializeField] protected UnityEvent onBreakInteraction;
    [SerializeField] protected UnityEvent onStopInteraction;
    [SerializeField] protected UnityEvent onFirstStopInteraction;
    
    protected bool isInteracting;
    public bool HasFirstInteracted {get; set;}
    public bool HasFirstStoppedInteraction {get; set;}
    
    public virtual void Interact()
    {
        isInteracting = true;
        onInteract.Invoke();
        
        if(!HasFirstInteracted)
        {
            onFirstInteract.Invoke();
        }
        
        HasFirstInteracted = true;
    }

    public virtual void StopInteraction()
    {
        isInteracting = false;
        onStopInteraction.Invoke();
        
        if(!HasFirstStoppedInteraction)
        {
            onFirstStopInteraction.Invoke();
        }
        
        HasFirstStoppedInteraction = true;
    }
    
    public virtual void BreakInteraction()
    {
        isInteracting = false;
        onBreakInteraction.Invoke();
    }
    
    public bool IsInteracting()
    {
        return isInteracting;
    }

    public string GetInteractText()
    {
        return interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }
    
    public abstract void Highlight(bool value);
    public abstract bool CanInteract();
}