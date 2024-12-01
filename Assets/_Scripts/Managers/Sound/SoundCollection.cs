using UnityEngine;

[CreateAssetMenu(fileName = "SoundCollection", menuName = "SoundCollection", order = 1)]
public class SoundCollection : ScriptableObject
{
    [field: Header("Word Sounds")]
    [field: SerializeField] public AudioClip[] WordSelect {get; private set;}
    [field: SerializeField] public AudioClip[] WordPlace {get; private set;}
    [field: SerializeField] public AudioClip[] WordMerge {get; private set;}
    [field: SerializeField] public AudioClip[] WordSplit {get; private set;}
    [field: SerializeField] public AudioClip[] WordCantMerge {get; private set;}
    [field: SerializeField] public AudioClip[] WordCantSplit {get; private set;}
    [field: SerializeField] public AudioClip[] CollectWord {get; private set;}
    
    [field: Header("Password Sounds")]
    [field: SerializeField] public AudioClip[] SlotCorrect {get; private set;}
    [field: SerializeField] public AudioClip[] SlotIncorrect {get; private set;}
    [field: SerializeField] public AudioClip[] PasswordCorrect {get; private set;}
    [field: SerializeField] public AudioClip[] PasswordIncorrect {get; private set;}
    [field: SerializeField] public AudioClip[] PlaceInSlot {get; private set;}
    [field: SerializeField] public AudioClip[] RemoveFromSlot {get; private set;}
    [field: SerializeField] public AudioClip[] NavigateThroughPasswords {get; private set;}
    
    [field: Header("UI Sounds")]
    [field: SerializeField] public AudioClip[] ButtonClick {get; private set;}
    [field: SerializeField] public AudioClip[] ButtonHoverStart {get; private set;}
    [field: SerializeField] public AudioClip[] ButtonHoverEnd {get; private set;}
    [field: SerializeField] public AudioClip[] OpenNotebookUI {get; private set;}
    [field: SerializeField] public AudioClip[] CloseNotebookUI {get; private set;}
    
    [field: Header("Conversation Sounds")]
    [field: SerializeField] public AudioClip[] ConversationStart {get; private set;}
    [field: SerializeField] public AudioClip[] ConversationEnd {get; private set;}
    [field: SerializeField] public AudioClip[] IndexHit {get; private set;}
    [field: SerializeField] public AudioClip[] IndexMiss {get; private set;}
    
    [field: Header("Step Sounds")]
    [field: SerializeField] public AudioClip[] StepsLevel1 {get; private set;}
    [field: SerializeField] public AudioClip[] StepsPrologue {get; private set;}
    
    public AudioClip[] GetCurrentLevelSteps()
    {
        return GameManager.Instance.LevelManager.CurrentLevel.LevelScene switch
        {
            SceneEnum.Prologue => StepsPrologue,
            SceneEnum.Level1 => StepsLevel1,
            _ => StepsPrologue
        };
    }
    
    [field: Header("Music")]
    [field: SerializeField] public AudioClip MainMenuMusic {get; private set;}
}