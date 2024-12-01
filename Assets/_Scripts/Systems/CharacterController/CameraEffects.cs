using AdvancedController;
using Unity.Cinemachine;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] private PlayerController playerController;
    
    [Header("Settings")]
    [SerializeField] private float noiseLerpSpeed = 10f;
    
    [Header("Movement Noise")]
    [SerializeField] private NoiseSettings movementNoise;
    [SerializeField] private float movementNoiseAmplitude = 0.1f;
    [SerializeField] private float movementNoiseFrequency = 0.7f;
    
    [Header("Idle Noise")]
    [SerializeField] private NoiseSettings idleNoise;
    [SerializeField] private float idleNoiseAmplitude = 0.15f;
    [SerializeField] private float idleNoiseFrequency = 0.15f;
    
    private float _currentAmplitude;
    private float _targetAmplitude;
    
    private float _stepSoundPeriod;
    
    private void Start()
    {
        _stepSoundPeriod = 1 / movementNoiseFrequency / 2f;
    }
    
    private void Update()
    {
        HandleNoise();
        HandleStepSounds();
    }

    private void HandleStepSounds()
    {
        if(!playerController.IsMoving) return;
        
        if (Time.time % _stepSoundPeriod < Time.deltaTime)
        {
            AudioManager.Instance.PlayStepSound(transform.position, _currentAmplitude/movementNoiseAmplitude);
        }
    }
    
    private void HandleNoise()
    {
        if (playerController.IsMoving)
        {
            if (noise.NoiseProfile != movementNoise)
            {
                _targetAmplitude = 0;
            }

            if (_currentAmplitude <= 0.01f)
            {
                _targetAmplitude = movementNoiseAmplitude;
                noise.NoiseProfile = movementNoise;
            }
        }
        else
        {
            if (noise.NoiseProfile != idleNoise)
            {
                _targetAmplitude = 0;
            }

            if (_currentAmplitude <= 0.01f)
            {
                _targetAmplitude = idleNoiseAmplitude;
                noise.NoiseProfile = idleNoise;
            }
        }
        
        _currentAmplitude = Mathf.Lerp(_currentAmplitude, _targetAmplitude, Time.deltaTime * noiseLerpSpeed);
        noise.AmplitudeGain = _currentAmplitude;

        noise.FrequencyGain = playerController.IsMoving ? movementNoiseFrequency : idleNoiseFrequency;
    }
}