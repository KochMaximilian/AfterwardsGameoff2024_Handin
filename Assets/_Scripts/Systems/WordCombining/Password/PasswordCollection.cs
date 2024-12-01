using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PasswordCollection : MonoBehaviour, ISceneInitializer
{
    public static event Action OnInitialized;
    
    [Header("References")]
    [SerializeField] private GameObject passwordPrefab;
    [SerializeField] private Transform passwordParent;
    
    private List<Password> _passwords;
    public IReadOnlyList<Password> Passwords => _passwords;

    private void Start()
    {
        _passwords = new List<Password>();
    }

    public void OnStartScene()
    {
        _passwords = new List<Password>();
        
        for (int i = 0; i < GameManager.Instance.LevelManager.CurrentLevel.Passwords.Count; i++)
        {
            Password password = Instantiate(passwordPrefab, passwordParent).GetComponent<Password>();
            password.Initialize( GameManager.Instance.LevelManager.CurrentLevel.Passwords[i], this);
            _passwords.Add(password);
        }
        
        OnInitialized?.Invoke();
    }
    
    public void CheckLevelComplete(Password password)
    {
        for (int i = 0; i < _passwords.Count; i++)
        {
            if(_passwords[i] == password) continue;
            if (_passwords[i].IsPasswordCorrect().Any(x => x != true))
            {
                return;
            }
        }

        GameManager.Instance.LevelManager.TransitionToNextLevel();
    }
}