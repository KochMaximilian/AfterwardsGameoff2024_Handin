using System;
using System.Collections.Generic;
using UnityEngine;

public class Password : MonoBehaviour
{
    public event Action OnInitialized; 
    
    [Header("References")]
    [SerializeField] private GameObject wordSlotPrefab;
    [SerializeField] private Transform wordSlotParent;
    
    [Header("Data")]
    private PasswordData _passwordData;
    
    private bool _initialized;
    private List<WordSlot> _wordSlots;
    private PasswordCollection _passwordCollection;
    
    public IReadOnlyList<PasswordFragment> Fragments => _passwordData.Fragments;
    public IReadOnlyList<WordSlot> Slots => _wordSlots;
    public string Name => _passwordData.Name;
    public bool Initialized => _initialized;
    
    public void Initialize(PasswordData passwordData, PasswordCollection passwordCollection)
    {
        _passwordData = passwordData;
        _passwordCollection = passwordCollection;
        _wordSlots = new List<WordSlot>();
        
        for (int i = 0; i < Fragments.Count; i++)
        {
            WordSlot wordSlot = Instantiate(wordSlotPrefab, wordSlotParent).GetComponent<WordSlot>();
            _wordSlots.Add(wordSlot);
        }
        
        _initialized = true;
        OnInitialized?.Invoke();
    }
    
    public List<bool> IsPasswordCorrect()
    {
        List<bool> result = new List<bool>();
        
        for (int i = 0; i < _wordSlots.Count; i++)
        {
            if (_wordSlots[i].Word == null)
            {
                result.Add(false);
                continue;
            }
                
            string targetWord = Fragments[i].UseText ? Fragments[i].wordText : Fragments[i].word.Text;
            if (_wordSlots[i].Word.WordNode.WordData.Text != targetWord)
            {
                result.Add(false);
                continue;
            }
            
            result.Add(true);
        }
        
        return result;
    }

    public void CheckPassword(List<bool> result)
    {
        if(!result.Contains(false)) _passwordCollection.CheckLevelComplete(this);
    }
}