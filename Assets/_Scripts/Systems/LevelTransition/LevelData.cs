using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level", order = 1)]
public class LevelData : ScriptableObject
{
    [Header("Passwords")]
    [SerializeField] private PasswordData[] passwords;
    
    [Header("Settings")]
    [SerializeField] private bool canUseNotebook = true;
    [SerializeField] private bool isTimerActive = true;
    
    [Header("Scenes")]
    [SerializeField] private SceneEnum levelScene;
    
    [Header("Music")]
    [SerializeField] private AudioClip music;
    
    public IReadOnlyList<PasswordData> Passwords => passwords;
    public bool CanUseNotebook => canUseNotebook;
    public SceneEnum LevelScene => levelScene;
    public bool IsTimerActive => isTimerActive;
    public AudioClip Music => music;
}