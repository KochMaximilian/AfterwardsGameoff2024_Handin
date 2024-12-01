using System;
using UnityEngine;
using UnityEngine.Events;

public class NotebookInteractionChecker : MonoBehaviour
{
    public void SetInteractedWithNotebook(bool interacted)
    {
        GameManager.Instance.HasFirstInteractedWithNotebook = interacted;
    }
}
