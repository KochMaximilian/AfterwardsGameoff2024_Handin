using UnityEngine;
using UnityUtils;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private SoundCollection soundCollection;
    [SerializeField] private float stepsVolume = 0.5f;
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource musicAudioSource;
    
    public SoundCollection Sfx => soundCollection;

    private AudioClip _lastSound;

    public void PlaySound(AudioClip clip)
    {
        if(clip == null) return;
        
        sfxAudioSource.PlayOneShot(clip);
    }
    
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = -1f)
    {
        if(clip == null) return;
        if(volume < 0) volume = sfxAudioSource.volume;
        
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
    
    public void PlaySoundAtPosition(AudioClip[] clips, Vector3 position, float volume = -1f)
    {
        if(clips == null || clips.Length <= 0) return;
        if(volume < 0) volume = sfxAudioSource.volume;

        AudioClip clip;
        do
        { 
            clip = clips[Random.Range(0, clips.Length)];
        } while (clips.Length > 1 && _lastSound == clip);
        
        _lastSound = clip;
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    public void PlaySound(string clipName)
    {
        AudioClip clip = (AudioClip) soundCollection.GetType().GetField(clipName).GetValue(soundCollection);
        PlaySound(clip);
    }
    
    public void PlaySound(AudioClip[] clips)
    {
        if(clips == null || clips.Length == 0) return;
        
        AudioClip clip;
        do
        { 
            clip = clips[Random.Range(0, clips.Length)];
        } while (clips.Length > 1 && _lastSound == clip);
        
        _lastSound = clip;
        PlaySound(clip);
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if(clip == null) return;
        
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }
    
    public void StopMusic()
    {
        musicAudioSource.Stop();
    }
    
    public void ToggleMusic()
    {
        if(musicAudioSource.isPlaying) musicAudioSource.Pause();
        else musicAudioSource.Play();
    }
    
    public void ToggleMute()
    {
        AudioListener.pause = !AudioListener.pause;
    }
    
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
    
    public void SetMusicVolume(float volume)
    {
        if(musicAudioSource == null) return;
        musicAudioSource.volume = volume;
    }

    public void PlayStepSound(Vector3 transformPosition, float volume)
    {
        AudioClip[] clips = soundCollection.GetCurrentLevelSteps();
        PlaySoundAtPosition(clips, transformPosition, volume*stepsVolume);
    }

    public float GetMusicVolume()
    {
        return musicAudioSource.volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxAudioSource.volume = volume;
    }
}