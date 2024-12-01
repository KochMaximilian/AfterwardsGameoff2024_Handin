using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public class InteractableObject : BaseInteractable, IDataHandler<InteractableObjectSnapshot>
{
    [SerializeField] private UnityEvent AfterLoadIfInteracted;
    [SerializeField] private bool canInteract = true;
    private Outline _outline;

    protected virtual void Start()
    {
        gameObject.isStatic = false;
        _outline = gameObject.GetComponent<Outline>();
        
        _outline.OutlineMode = Outline.Mode.OutlineAll;
        _outline.OutlineWidth = 5f;
        _outline.enabled = false;
    }

    public override void Highlight(bool value)
    {
        if(_outline == null) return;
        _outline.enabled = value;
    }
    
    public override bool CanInteract()
    {
        return canInteract;
    }
    
    public void SetCanInteract(bool value)
    {
        canInteract = value;
    }
    
    public virtual InteractableObjectSnapshot GetData()
    {
        InteractableObjectSnapshot data = new InteractableObjectSnapshot();
        data.canInteract = canInteract;
        data.hasFirstInteracted = HasFirstInteracted;
        data.hasFirstStoppedInteraction = HasFirstStoppedInteraction;
        data.interactText = interactText;
        data.hash = GetHash();

        return data;
    }

    public virtual void LoadData(InteractableObjectSnapshot data)
    {
        canInteract = data.canInteract;
        HasFirstInteracted = data.hasFirstInteracted;
        HasFirstStoppedInteraction = data.hasFirstStoppedInteraction;
        interactText = data.interactText;
        
        if(HasFirstInteracted)
        {
            AfterLoadIfInteracted.Invoke();
        }
    }

    public long GetHash()
    {
        return interactText.GetHashCode() + name.GetHashCode();
    }
}