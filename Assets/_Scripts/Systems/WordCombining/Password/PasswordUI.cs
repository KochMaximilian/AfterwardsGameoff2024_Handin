using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Password password;
    
    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private RectTransform horizontalLayoutGroup;
    [SerializeField] private Button confirmButton;
    
    private void Awake()
    {
        password.OnInitialized += Initialize;
        if(password.Initialized) Initialize();
    }

    private void Initialize()
    {
        titleText.text = password.Name;
        RectTransform rt = titleText.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(titleText.preferredWidth, titleText.preferredHeight);
        rt.anchoredPosition = new Vector2(titleText.preferredWidth / 2, 0);
        
        horizontalLayoutGroup.sizeDelta = new Vector2(horizontalLayoutGroup.sizeDelta.x - rt.sizeDelta.x, horizontalLayoutGroup.sizeDelta.y);
        horizontalLayoutGroup.anchoredPosition = new Vector2(horizontalLayoutGroup.anchoredPosition.x + titleText.preferredWidth/2f, 0);
        
        confirmButton.onClick.AddListener(() =>
        {
            List<bool> result = password.IsPasswordCorrect();
            OnPasswordChanged(result);
            password.CheckPassword(result);
            
            bool isCorrect = !result.Contains(false);
            WordSounds.PlayPasswordSound(isCorrect);
            if (isCorrect) confirmButton.interactable = false;
            else Timer.Instance.Penalize(5f);
        });
        
       
        foreach (WordSlot slot in password.Slots)
        {
            slot.OnChange += OnSlotChanged;
        } 
        
        password.OnInitialized -= Initialize;
    }
    
    private void OnPasswordChanged(List<bool> result)
    {
        for (int i = 0; i < password.Slots.Count; i++)
        {
            if(result[i]) password.Slots[i].OnCorrect();
            else password.Slots[i].OnIncorrect();
        }
    }
    
    private void OnSlotChanged()
    {
        List<bool> result = password.IsPasswordCorrect();
        
        if (!confirmButton.interactable && result.Contains(false))
            confirmButton.interactable = true;
    }

    private void OnDestroy()
    {
        foreach (WordSlot slot in password.Slots)
        {
            slot.OnChange -= OnSlotChanged;
        }
    }
}