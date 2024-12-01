using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WordSlot : MonoBehaviour
{
    public event Action OnChange;
    
    [SerializeField] private RectTransform slotRect;
    [SerializeField] private ParticleSystem correctParticles;
    
    private Word _word;
    private bool _isOccupied;
    
    public bool IsOccupied => _isOccupied;
    public Word Word => _word;
    
    public void OnCorrect()
    {
        var main = correctParticles.main;
        main.startColor = WordVfxManger.Instance.GetColor(_word.WordNode.Depth);
        
        WordSounds.PlaySlotSound(true);
        correctParticles.Play();
    }
    
    public void OnIncorrect()
    {
        transform.DOComplete(true);
        Vector3 initialPosition = transform.position;

        WordSounds.PlaySlotSound(false);
        transform.DOShakePosition(0.25f, 10, 100).OnComplete(() => transform.position = initialPosition);
    }

    public void SwapWords(WordSlot otherSlot)
    {
        Word[] words = {_word, otherSlot.Word};
        otherSlot.Word.Mover.Move(transform.position, false, () => PlaceWord(words[1]));
        _word.Mover.Move(otherSlot.transform.position, false, () => otherSlot.PlaceWord(words[0]));
    }
    
    public void ReplaceWord(Word word, Vector3 initialPosition)
    {
        if(!IsOccupied) return;
        Word myWord = _word;
        RemoveWord();
        myWord.Mover.Move(initialPosition);
        
        PlaceWord(word);
    }
    
    public void PlaceWord(Word word)
    {
        _isOccupied = true;
        _word = word;
        _word.Slot = this;
        
        word.transform.position = transform.position;
        word.transform.SetParent(transform);
        word.Visuals.AdjustSize(slotRect.rect.width, slotRect.rect.height);
        OnChange?.Invoke();
    }
    
    public void RemoveWord()
    {
        _word.transform.SetParent(WordManager.Instance.WordContainer);
        _word.Visuals.AdjustSize();
        
        _word.Slot = null;
        _word = null;
        _isOccupied = false;
        OnChange?.Invoke();
    }

    public void PreviewRemoveWord()
    {
        WordSounds.PlayRemoveFromSlotSound();
        
        _word.transform.SetParent(WordManager.Instance.WordContainer);
        _word.Visuals.AdjustSize();
        
        Word myWord = _word;
        _word = null;
        OnChange?.Invoke();
        
        _isOccupied = false;
        _word = myWord;
    }
}