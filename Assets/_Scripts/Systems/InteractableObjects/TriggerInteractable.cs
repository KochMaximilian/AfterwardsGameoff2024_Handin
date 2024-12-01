using UnityEngine;

public class TriggerInteractable : DialogueInteractable
{
    [Header("Trigger Settings")]
    [SerializeField] private Collider triggerCollider;
    
    private bool _triggered;

    protected override void Start()
    {
        triggerCollider.isTrigger = true;
        
        Outline outline = gameObject.AddComponent<Outline>();
        if(outline != null)
            Destroy(outline);
    }

    public override void Interact()
    {
        base.Interact();
        _triggered = true;
    }
    
    public override bool CanInteract()
    {
        return base.CanInteract() && !_triggered;
    }

    public override void Highlight(bool value)
    {
        // Do nothing
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && CanInteract())
        {
            Interact();
        }
    }
}