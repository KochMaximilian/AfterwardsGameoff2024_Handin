using UnityEngine;

public static class WordSounds
{
    public static void PlaySelectSound()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.WordSelect);
    }
    
    public static void PlayMergeSound(bool merged)
    {
        AudioClip[] clip = merged ? AudioManager.Instance.Sfx.WordMerge : AudioManager.Instance.Sfx.WordCantMerge;
        AudioManager.Instance.PlaySound(clip);
    }
    
    public static void PlaySplitSound(bool split)
    {
        AudioClip[] clip = split ? AudioManager.Instance.Sfx.WordSplit : AudioManager.Instance.Sfx.WordCantSplit;
        AudioManager.Instance.PlaySound(clip);
    }
    
    public static void PlayPlaceSound(bool inSlot)
    {
        AudioClip[] clip = inSlot ? AudioManager.Instance.Sfx.PlaceInSlot : AudioManager.Instance.Sfx.WordPlace;
        AudioManager.Instance.PlaySound(clip);
    }
    
    public static void PlayRemoveFromSlotSound()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.RemoveFromSlot);
    }
    
    public static void PlayCollectWordSound()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.CollectWord);
    }
    
    public static void PlaySlotSound(bool correct)
    {
        AudioClip[] clip = correct ? AudioManager.Instance.Sfx.SlotCorrect : AudioManager.Instance.Sfx.SlotIncorrect;
        AudioManager.Instance.PlaySound(clip);
    }
    
    public static void PlayPasswordSound(bool correct)
    {
        AudioClip[] clip = correct ? AudioManager.Instance.Sfx.PasswordCorrect : AudioManager.Instance.Sfx.PasswordIncorrect;
        AudioManager.Instance.PlaySound(clip);
    }
    
    public static void PlayNavigateThroughPasswordsSound()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.Sfx.NavigateThroughPasswords);
    }
}