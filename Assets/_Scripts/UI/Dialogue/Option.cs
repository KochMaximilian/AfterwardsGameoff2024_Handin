using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Option : BaseInteractable
{
    [Header("Option Settings")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image background;

    private DialogueOption _option;
    private bool _isSelected;
    
    public bool IsSelected => _isSelected;

    public void Initialize(DialogueOption option)
    {
        _option = option;
        text.text = option.text;
        interactText = option.text;
        
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        RectTransform rt = gameObject.GetComponentInParent<RectTransform>();
        collider.size = new Vector3(rt.sizeDelta.x, rt.sizeDelta.y, 1);
    }

    public override void Interact()
    {
        base.Interact();
        _isSelected = true;
        DialogueManager.Instance.SelectOption(_option);
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.ButtonClick);
        _option.Select();
    }

    public override void Highlight(bool value)
    {
        float alpha = value ? 0.2f : 0f;
        background.color = new Color(background.color.r, background.color.g, background.color.b, alpha);
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.ButtonHoverStart);
    }

    public override bool CanInteract()
    {
        return true;
    }
}