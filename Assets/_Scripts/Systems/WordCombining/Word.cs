using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(WordVisuals))]
public class Word : MonoBehaviour
{
    #region Fields

    [SerializeField] private TMP_Text text;
    
    private WordNode _wordNode;
    private WordVisuals _wordVisuals;
    private WordMover _wordMover;
    private Rect _wordRect;
    private Vector3 _previousPosition;

    #endregion

    #region Properties

    public WordSlot Slot { get; set; }

    public bool InSlot => Slot != null;
    public WordNode WordNode => _wordNode;
    public WordVisuals Visuals => _wordVisuals;
    public Rect Rect => _wordRect;
    
    public WordMover Mover => _wordMover;

    #endregion
    
    #region Initialization
    
    public void Initialize(WordNode wordNode)
    {
        _wordVisuals = GetComponent<WordVisuals>();
        _wordMover = new WordMover(GetComponent<RectTransform>());
        
        _wordNode = wordNode;
        text.text = wordNode.WordData.Text;

        _wordVisuals.Initialize(WordVfxManger.Instance.GetColor(wordNode.Depth));
        _wordRect = GetComponent<RectTransform>().rect;
    }

    #endregion

    #region Events

    public async UniTask<bool> OnReleaseAsync(GameObject otherObject)
    {
        _wordVisuals.Select(false);
        _wordMover.ChangeToWordSpace();
        _wordVisuals.Hover(false);

        if (otherObject == null)
        {
            WordSounds.PlayPlaceSound(false);

            await _wordMover.MoveAsync(transform.position);
            if (InSlot) Slot.RemoveWord();
            return false;
        }

        if(otherObject.TryGetComponent(out WordSlot wordSlot))
        {
            WordSounds.PlayPlaceSound(true);

            if (InSlot) Slot.RemoveWord();
             wordSlot.PlaceWord(this);
        }
        else if(otherObject.TryGetComponent(out Word otherWord))
        {
            //If the other word is in a slot, we swap the words
            if (otherWord.InSlot && InSlot)
            {
                Slot.SwapWords(otherWord.Slot);
                return false;
            }
            
            //If the other word is in a slot, we replace the word in the slot with this word
            if(otherWord.InSlot)
            {
                otherWord.Slot.ReplaceWord(this, _previousPosition);
                return false;
            }
            
            Word mergedWord = await WordManager.Instance.Merge(otherWord, this);
            if (mergedWord == null)
            {
                WordSounds.PlayMergeSound(false);
                _wordVisuals.Shake();
                otherWord.Visuals.Shake();
            }
        }
        
        return true;
    }
    
    public async UniTask<bool> OnSplitAsync()
    {
        if(InSlot) return false;
        
        _wordVisuals.Select(false);
        bool split = await WordManager.Instance.TrySplit(this);

        if (!split)
        {
            WordSounds.PlaySplitSound(false);
            _wordVisuals.Shake();
        }
        return split;
    }
    
    public void OnStartSelecting()
    {
        if(InSlot) Slot.PreviewRemoveWord();
        _previousPosition = transform.position;
        
        _wordMover.ChangeToMoveSpace();
        _wordVisuals.Select(true);
        
        WordSounds.PlaySelectSound();
    }
    
    public void OnBeingSelected(Vector2 pointerPosition)
    {
        transform.position = _wordMover.TransformPoint(pointerPosition);
    }
    
    public void OnPointerEnter()
    {
        if(InSlot) return;
        _wordVisuals.Hover(true);
    }
    
    public void OnPointerExit()
    {
        if(InSlot) return;
        _wordVisuals.Hover(false);
    }


    #endregion

    #region Overrides

    public override bool Equals(object other)
    {
        return other is Word word && _wordNode.WordData.Text == word._wordNode.WordData.Text;
    }
    
    public override int GetHashCode()
    {
        return _wordNode.WordData.Text.GetHashCode();
    }

    #endregion
}