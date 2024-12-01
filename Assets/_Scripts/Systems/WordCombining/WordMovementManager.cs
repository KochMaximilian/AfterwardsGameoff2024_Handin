using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils;
using Random = UnityEngine.Random;

public class WordMovementManager : Singleton<WordMovementManager>
{
    [Header("References")]
    [SerializeField] private RectTransform movementSpace;
    [SerializeField] private RectTransform wordContainer;
    
    [Header("Data")]
    [SerializeField] private float moveSpeed = 20f;
    
    public RectTransform WordContainer => wordContainer;
    public RectTransform MovementSpace => movementSpace;

    protected override void Awake()
    {
        base.Awake();
        
        wordContainer.pivot = new Vector2(0, 0);
        movementSpace.pivot = new Vector2(0, 0);
        
        wordContainer.ForceUpdateRectTransforms();
        movementSpace.ForceUpdateRectTransforms();
    }
    
    public async UniTask MoveToAsync(RectTransform wordTransform, Vector3 targetPosition, bool checkInBounds = true, Action onComplete = null)
    {
        wordTransform.SetParent(movementSpace);
        
        if (checkInBounds)
            targetPosition = GetClosestInBoundsPoint(wordContainer, wordTransform.rect, targetPosition);
        
        float duration = GetMoveDuration(wordTransform.position, targetPosition);
        await wordTransform.DOMove(targetPosition, duration).AsyncWaitForCompletion();
        
        onComplete?.Invoke();
    }

    public void MoveTo(RectTransform wordTransform, Vector3 targetPosition, bool checkInBounds = true, Action onComplete = null)
    {
        wordTransform.SetParent(movementSpace);
        
        if (checkInBounds)
            targetPosition = GetClosestInBoundsPoint(wordContainer, wordTransform.rect, targetPosition);
        
        float duration = GetMoveDuration(wordTransform.position, targetPosition);
        wordTransform.DOMove(targetPosition, duration).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
    
    public void InstantMoveTo(RectTransform wordTransform, Vector3 targetPosition, bool checkInBounds = true)
    {
        if (checkInBounds)
            targetPosition = GetClosestInBoundsPoint(wordContainer, wordTransform.rect, targetPosition);
        wordTransform.position = targetPosition;
    }
    
    private float GetMoveDuration(Vector3 startPosition, Vector3 targetPosition)
    {
        return Vector3.Distance(startPosition, targetPosition) / moveSpeed;
    }
    
    public bool IsOutOfBounds(RectTransform container, Rect wordRect, Vector3 wordPosition)
    {
        wordPosition = container.InverseTransformPoint(wordPosition);
        
        if (!container.rect.Contains(wordPosition + (Vector3)wordRect.size / 2f)) return true;
        if (!container.rect.Contains(wordPosition - (Vector3)wordRect.size / 2f)) return true;
        
        return false;
    }
    
    public bool IsOutOfBounds(Rect wordRect, Vector3 wordPosition)
    {
        return IsOutOfBounds(wordContainer, wordRect, wordPosition);
    }

    public Vector3 GetClosestInBoundsPoint(RectTransform container, Rect wordRect, Vector3 wordPosition)
    {
        if(!IsOutOfBounds(container, wordRect, wordPosition)) return wordPosition;
        
        wordPosition = container.InverseTransformPoint(wordPosition);
        wordPosition.z = 0;
        Vector3 closestPoint = wordPosition;
        
        if (wordPosition.x - wordRect.width / 2f < 0) closestPoint.x = wordRect.width / 2f;
        if (wordPosition.x + wordRect.width / 2f > container.rect.width) closestPoint.x = container.rect.width - wordRect.width / 2f;
        
        if (wordPosition.y - wordRect.height / 2f < 0) closestPoint.y = wordRect.height / 2f;
        if (wordPosition.y + wordRect.height / 2f > container.rect.height) closestPoint.y = container.rect.height - wordRect.height / 2f;

        return container.TransformPoint(closestPoint);
    }
    
    public Vector3 GetClosestInBoundsPoint(Rect wordRect, Vector3 wordPosition)
    {
        return GetClosestInBoundsPoint(wordContainer, wordRect, wordPosition);
    }
    
    public Vector3 GetRandomPositionInBounds(RectTransform container, Rect wordRect)
    {
        Vector2 pivot = container.pivot;
        container.pivot = new Vector2(0f, 0f);
        
        Vector3 randomPosition = new Vector3(Random.Range(wordRect.width / 2f, container.rect.width - wordRect.width / 2f), 
            Random.Range(wordRect.height / 2f, container.rect.height - wordRect.height / 2f));

        container.pivot = pivot;
        randomPosition.z = 0;
        Vector3 position = container.TransformPoint(randomPosition);
        
        return position;
    }
    
    public Vector3 GetRandomPositionInBounds(Rect wordRect)
    {
        return GetRandomPositionInBounds(wordContainer, wordRect);
    }
}