using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WordMover
{
    private RectTransform _rectTransform;
    
    public WordMover(RectTransform rectTransform)
    {
        _rectTransform = rectTransform;
    }
    
    public void ChangeToMoveSpace()
    {
        _rectTransform.SetParent(WordMovementManager.Instance.MovementSpace);
    }
    
    public void ChangeToWordSpace()
    {
        _rectTransform.SetParent(WordMovementManager.Instance.WordContainer);
    }
    
    public async UniTask MoveAsync(Vector3 targetPosition, bool checkInBounds = true, Action onComplete = null)
    {
        if(onComplete == null) onComplete = ChangeToWordSpace;
        await WordMovementManager.Instance.MoveToAsync(_rectTransform, targetPosition, checkInBounds, onComplete);
    }
    
    public void Move(Vector3 targetPosition, bool checkInBounds = true, Action onComplete = null)
    {
        if(onComplete == null) onComplete = ChangeToWordSpace;
        WordMovementManager.Instance.MoveTo(_rectTransform, targetPosition, checkInBounds, onComplete);
    }

    public Vector3 GetInBoundsRandomPosition()
    {
        return WordMovementManager.Instance.GetRandomPositionInBounds(_rectTransform.rect);
    }
    
    public Vector3 TransformPoint(Vector3 position)
    {
        return WordMovementManager.Instance.MovementSpace.TransformPoint(position);
    }
}