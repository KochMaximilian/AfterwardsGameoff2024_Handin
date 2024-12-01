using UnityEngine;

public interface IInteractable
{
   void Interact();
   void StopInteraction();
   void BreakInteraction();
   bool IsInteracting();
   string GetInteractText();
   Transform GetTransform();
   
   bool CanInteract();
   void Highlight(bool value);
}