using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Word))]
public class WordVisuals : MonoBehaviour
{
    //TODO: Move this data to scriptable object and Rely on an interface for the visuals
    [Header("Scales")]
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float selectedScale = 1.25f;
    
    [Header("Durations")]
    [SerializeField] private float appearDuration = 0.125f;
    [SerializeField] private float disappearDuration = 0.125f;
    [SerializeField] private float hoverDuration = 0.06125f;
    [SerializeField] private float shakeDuration = 0.15f;
    
    private TextFitter _textFitter;
    private Image _image;
    
    private bool _duringAnimation;
    public bool DuringAnimation => _duringAnimation;
    
    public void Initialize(Color getColor)
    {
        _textFitter = GetComponent<TextFitter>();
        _image = GetComponent<Image>();
        
        SetColor(getColor);
        AdjustSize();
    }
    
    public async UniTask AppearAsync()
    {
        _duringAnimation = true;
        
        transform.localScale = Vector3.zero;
        await transform.DOScale(Vector3.one, appearDuration).AsyncWaitForCompletion();
        
        _duringAnimation = false;
    }
    
    public async UniTask AppearAsync(Vector3 position, Vector3 finalPosition)
    {
        _duringAnimation = true;
        
        transform.position = position;
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, appearDuration);
        await transform.DOMove(finalPosition, appearDuration).AsyncWaitForCompletion();
        
        _duringAnimation = false;
    }
    
    public async UniTask DisappearAsync()
    {
        _duringAnimation = true;

        await transform.DOScale(Vector3.zero, disappearDuration).AsyncWaitForCompletion();

        _duringAnimation = false;
    }
    
    public void Hover(bool isHovering)
    {
        if (_duringAnimation) return;
        transform.DOScale(isHovering ? hoverScale * Vector3.one : normalScale * Vector3.one, hoverDuration);
    }
    
    public void Select(bool isSelected)
    {
        if (_duringAnimation) return;
        transform.localScale = isSelected ? selectedScale * Vector3.one : hoverScale * Vector3.one;
    }
    
    
    public void AdjustSize(float targetWidth, float targetHeight)
    {
        _textFitter.AdjustSize(targetWidth, targetHeight);
    }
    
    public void AdjustSize()
    {
        _textFitter.AdjustSize();
    }

    private void SetColor(Color color)
    {
        _image.color = color;
    }

    public void Shake()
    {
        if(_duringAnimation) return;
        _duringAnimation = true;
        
        Vector3 originalPosition = transform.position;
        transform.DOShakePosition(shakeDuration, 10, 50).OnComplete(() =>
        {
            _duringAnimation = false;
            transform.position = originalPosition;
        });
    }

    public async UniTask ShakeAsync()
    {
        if(_duringAnimation) return;
        _duringAnimation = true;
        
        Vector3 originalPosition = transform.position;
        await transform.DOShakePosition(shakeDuration, 10, 50).AsyncWaitForCompletion();
        
        _duringAnimation = false;
        transform.position = originalPosition;
    }
}