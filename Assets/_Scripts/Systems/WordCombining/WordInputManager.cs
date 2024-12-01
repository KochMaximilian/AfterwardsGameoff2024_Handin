using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WordInputManager : MonoBehaviour, ISceneInitializer
{
    [SerializeField] private Canvas canvas;
    
    private Word _selectedWord;
    private List<RaycastResult> _results;
    private bool _wasPointerPressed;

    private Word SelectedWord
    {
        get => _selectedWord;
        set => SetSelectedWord(value);
    }

    public bool IsInputDisabled { get; set; } = true;

    public void OnStartScene()
    {
        _results = new List<RaycastResult>();
    }
   
    
    private void Update()
    {
        if(!ConditionValidator.CanUseNotebook) return;
        if(IsInputDisabled) return;
        Pointer pointer = Pointer.current;
        
        if (pointer == null) return;

        HandlePointerAsync(pointer).Forget(); 
    }

    private async UniTask HandlePointerAsync(Pointer pointer)
    {
        if (SelectedWord != null)
        {
            if (InputManager.Instance.LeftClick.IsPressed())
            {
                if (!_wasPointerPressed)
                {
                    SelectedWord.OnStartSelecting();
                    _wasPointerPressed = true;
                }
                
                Vector3 position = pointer.position.ReadValue();
                position /= canvas.scaleFactor;
                SelectedWord.OnBeingSelected(position);
                return;
            }

            if (_wasPointerPressed)
            {
                _wasPointerPressed = false;

                if (await SelectedWord.OnReleaseAsync(GetWordOrSlotInPosition(pointer.position.ReadValue(), true)))
                {
                    SelectedWord = null;
                    return;
                }
            }

            if (InputManager.Instance.RightClick.WasPressedThisFrame())
            {
                if(SelectedWord.InSlot) return;

                if (await _selectedWord.OnSplitAsync())
                {
                    SelectedWord = null;
                    return;
                }
            }
        }

        GameObject wordOrSlot = GetWordOrSlotInPosition(pointer.position.ReadValue(), false);
        if (wordOrSlot == null)
        {
            SelectedWord = null;
            return;
        }
        
        SelectedWord = wordOrSlot.TryGetComponent<Word>(out var word) ? word : null;
    }
    
    private GameObject GetWordOrSlotInPosition(Vector2 position, bool ignoreSelectedWord)
    {
        //TODO: Refactor to use an interface to check if the object is a word or a slot
        var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = position,
        };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, _results);
        
        if (_results.Count == 0) return null;
        
        foreach (RaycastResult result in _results)
        {
            if (result.gameObject.TryGetComponent<WordSlot>(out var wordSlot))
            {
                if(wordSlot.IsOccupied) continue;
                return wordSlot.gameObject;
            }
            
            if (result.gameObject.TryGetComponent<Word>(out var word))
            {
                if (ignoreSelectedWord && word == _selectedWord) continue;
                if (word.Visuals.DuringAnimation) continue;
                return word.gameObject;
            }
        }

        return null;
    }
    
    private void SetSelectedWord(Word word)
    {
        if (_selectedWord != null && !_selectedWord.Equals(word))
        {
            if(word != null)
            {
                _selectedWord.OnPointerExit();
                word.OnPointerEnter();
            }
            else _selectedWord.OnPointerExit();
        }
        else if (_selectedWord == null && word != null)
        {
            word.OnPointerEnter();
        }
        
        _selectedWord = word;
    }
}