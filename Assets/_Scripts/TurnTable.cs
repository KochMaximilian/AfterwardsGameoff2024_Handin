using System;
using UnityEngine;

public class TurnTable : MonoBehaviour
{
    public float attenuationFactor = 1.0f;
    private bool _active = false;

    private AudioSource _audioSource;
    
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        Play();
        Pause();
    }

    public void Toggle()
    {
        if(!_active) Play();
        else Pause();
    }
    
    public void Play()
    {
        _active = true;
        _audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.4f);
        _audioSource.Play();
    }
    
    public void Pause()
    {
        _active = false;
        _audioSource.Pause();
        AudioManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.4f));
    }

    private void Update()
    {
        if(!_active) return;
        
        float distance = Vector3.Distance(CameraController.Instance.transform.position, transform.position);
        float volume = 1f / (1f + attenuationFactor * distance * distance);
        volume = 1 - Mathf.Clamp01(volume);

        AudioManager.Instance.SetMusicVolume(volume);
    }

    private void OnDisable()
    {
        AudioManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.4f));
    }

    private void OnDestroy()
    {
        AudioManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.4f));
    }
}