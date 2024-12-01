using System;
using DG.Tweening;
using UnityEngine;

public class PasswordNavigationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PasswordCollection passwordCollection;
    
    [Header("Settings")]
    [SerializeField] private float animationTime = 0.5f;
    
    private int _currentPasswordIndex;
    private int NextIndex => (_currentPasswordIndex + 1) % passwordCollection.Passwords.Count;
    private int PrevIndex => (_currentPasswordIndex - 1 + passwordCollection.Passwords.Count) % passwordCollection.Passwords.Count;
    
    public PasswordCollection PasswordCollection => passwordCollection;
    
    public event Action OnAnimationFinished;

    private void Awake()
    {
        PasswordCollection.OnInitialized += Initialize;
    }
    
    private void OnDestroy()
    {
        PasswordCollection.OnInitialized -= Initialize;
    }

    private void Initialize()
    {
        _currentPasswordIndex = 0;
        
        for (int i = 0; i < passwordCollection.Passwords.Count; i++)
        {
            passwordCollection.Passwords[i].gameObject.SetActive(i == _currentPasswordIndex);
        }
        
    }

    public void NavigateToNextPassword()
    {
        Transform next = passwordCollection.Passwords[NextIndex].transform;
        Navigate(false, next);
        _currentPasswordIndex = NextIndex;
    }
    
    public void NavigateToPrevPassword()
    {
        Transform prev = passwordCollection.Passwords[PrevIndex].transform;
        Navigate(true, prev);
        _currentPasswordIndex = PrevIndex;
    }

    private void Navigate(bool leftToRight, Transform next)
    {
        Transform current = passwordCollection.Passwords[_currentPasswordIndex].transform;
        Rect currentRect = current.GetComponent<RectTransform>().rect;
        currentRect.width *= 1.5f;
        float sign = leftToRight ? -1 : 1;

        next.gameObject.SetActive(true);
        next.SetAsFirstSibling();
        next.transform.localPosition = new Vector3(currentRect.width * sign, 0, 0);
        
        next.DOLocalMove(Vector3.zero, animationTime).OnComplete(() =>
        {
            OnAnimationFinished?.Invoke();
        });
        
        current.DOLocalMove(new Vector3(currentRect.width * -sign, 0, 0), animationTime).OnComplete(() =>
        {
            current.gameObject.SetActive(false);
        });
    }
}