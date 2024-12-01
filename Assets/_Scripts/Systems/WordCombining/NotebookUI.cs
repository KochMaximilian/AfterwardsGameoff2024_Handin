using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class NotebookUI : MonoBehaviour, ISceneInitializer
{
    [Header("References")]
    [SerializeField] private GameObject wordCombiningCanvas;
    [SerializeField] private GameObject[] notebookVisuals;
    [SerializeField] private WordInputManager wordInputManager;

    [Header("Settings")]
    [SerializeField] private float transitionDuration = 0.5f;
    
    private bool _isAnimating;
    private bool _isShowingNotebook;
    private NotebookInfoUI _notebookInfoUI;

    private void Start()
    {
        _notebookInfoUI = gameObject.GetComponent<NotebookInfoUI>();
    }

    public void OnStartScene()
    {
        if (wordCombiningCanvas == null) return;

        Cursor.visible = _isShowingNotebook;
        Cursor.lockState = _isShowingNotebook ? CursorLockMode.None : CursorLockMode.Locked;

        foreach (GameObject notebookVisual in notebookVisuals)
        {
            notebookVisual.SetActive(false);
        }
        wordInputManager.IsInputDisabled = true;
    }

    private void Update()
    {
        if(InputManager.Instance.OpenMenu.WasPressedThisFrame() && ConditionValidator.CanUseNotebook)
        {
            ShowNotebook(!_isShowingNotebook);
            
            if(_isShowingNotebook) InputManager.Instance.LookInputAction.Disable();
            else InputManager.Instance.LookInputAction.Enable();
        }
    }

    private void ShowNotebook(bool value)
    {
        if(_isAnimating) return;
        
        if(!value)
        {
            Debug.Log(_notebookInfoUI);
            _notebookInfoUI?.HideInfoPanel();
        }

        AudioClip[] clips = value ? AudioManager.Instance.Sfx.OpenNotebookUI : AudioManager.Instance.Sfx.CloseNotebookUI;
        AudioManager.Instance.PlaySound(clips);
        
        _isShowingNotebook = value;
        _isAnimating = true;
        foreach (GameObject notebookVisual in notebookVisuals)
        {
            notebookVisual.SetActive(true);
        }
        
        //Animate the canvas
        transform.localScale = _isShowingNotebook ? Vector3.zero : Vector3.one;
        Vector3 targetScale = _isShowingNotebook ? Vector3.one : Vector3.zero;
        Ease ease = _isShowingNotebook ? Ease.OutBack : Ease.InBack ;
        transform.DOScale(targetScale, transitionDuration).SetEase(ease).OnComplete(() => { 
            foreach (GameObject notebookVisual in notebookVisuals)
            {
                notebookVisual.SetActive(_isShowingNotebook);
            }
            
            transform.localScale = Vector3.one; 
            _isAnimating = false;
        });
        
        //Hide or show the cursor
        Cursor.visible = _isShowingNotebook;
        Cursor.lockState = _isShowingNotebook ? CursorLockMode.None : CursorLockMode.Locked;
        
        //Enable or disable the input manager
        wordInputManager.IsInputDisabled = !_isShowingNotebook;
    }
}