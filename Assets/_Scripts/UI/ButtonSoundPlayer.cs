using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundPlayer : MonoBehaviour
{
    private Button _button;
    private EventTrigger _eventTrigger;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _eventTrigger = gameObject.AddComponent<EventTrigger>();
        
        _button.onClick.AddListener(PlayClickSound);
        
        EventTrigger.Entry enterEntry = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
        enterEntry.callback.AddListener(_ => PlayEnterHoverSound());
        _eventTrigger.triggers.Add(enterEntry);
        
        EventTrigger.Entry exitEntry = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
        exitEntry.callback.AddListener(_ => PlayExitHoverSound());
        _eventTrigger.triggers.Add(exitEntry);
    }

    private void PlayClickSound()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.ButtonClick);
    }

    private void PlayEnterHoverSound()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.ButtonHoverStart);
    }

    private void PlayExitHoverSound()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.ButtonHoverEnd);
    }
}
