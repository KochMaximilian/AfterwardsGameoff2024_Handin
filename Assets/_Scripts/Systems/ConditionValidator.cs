using UnityEngine;

public static class ConditionValidator
{
    public static bool CanDisplayDialogue => !DialogueManager.Instance.IsDialogueActive
                                             && WordManager.Instance.WordInputManager.IsInputDisabled
                                             && !GameManager.Instance.IsPaused;

    public static bool CanUseNotebook => !DialogueManager.Instance.IsDialogueActive
                                         && GameManager.Instance.CanUseNotebook
                                         && GameManager.Instance.LevelManager.CurrentLevelIndex >= 0
                                         && GameManager.Instance.LevelManager.CurrentLevel.CanUseNotebook
                                         && !GameManager.Instance.IsPaused
                                         && GameManager.Instance.HasFirstInteractedWithNotebook;
}