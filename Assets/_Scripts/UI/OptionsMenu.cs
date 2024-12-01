using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils;

public class OptionsMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Button closeButton;
    
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text musicVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;
    
    [Header("Sensitivity")]
    [SerializeField] private Slider mouseSensitivityXSlider;
    [SerializeField] private Slider mouseSensitivityYSlider;
    [SerializeField] private TMP_Text mouseSensitivityXText;
    [SerializeField] private TMP_Text mouseSensitivityYText;
    
    public bool IsOptionsPanelActive => optionsPanel.activeInHierarchy;
    
    private void Start()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.4f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.6f));
        
        SetMouseSensitivityX(PlayerPrefs.GetFloat("MouseSensitivityX", 5));
        SetMouseSensitivityY(PlayerPrefs.GetFloat("MouseSensitivityY", 5));
        
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.4f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.6f);
        
        mouseSensitivityXSlider.value = PlayerPrefs.GetFloat("MouseSensitivityX", 5);
        mouseSensitivityYSlider.value = PlayerPrefs.GetFloat("MouseSensitivityY", 5);
        
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        
        mouseSensitivityXSlider.onValueChanged.AddListener(SetMouseSensitivityX);
        mouseSensitivityYSlider.onValueChanged.AddListener(SetMouseSensitivityY);
        
        closeButton.onClick.AddListener(HideOptionsPanel);
        
        Application.targetFrameRate = 60;
    }
    
    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }

    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        AudioManager.Instance.SetVolume(volume);
        
        masterVolumeText.text = volume.ToString("0.00");
    }
    
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        AudioManager.Instance.SetMusicVolume(volume);
        
        musicVolumeText.text = volume.ToString("0.00");
    }
    
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        AudioManager.Instance.SetSFXVolume(volume);
        
        sfxVolumeText.text = volume.ToString("0.00");
    }
    
    public void SetMouseSensitivityX(float sensitivity)
    {
        if(sensitivity < 0.1f) sensitivity = 0.1f;
        
        PlayerPrefs.SetFloat("MouseSensitivityX", sensitivity);
        
        if(CameraController.HasInstance)
            CameraController.Instance.SetSensitivityX(sensitivity);
        
        mouseSensitivityXText.text = sensitivity.ToString("00.0");
    }
    
    public void SetMouseSensitivityY(float sensitivity)
    {
        if(sensitivity < 0.1f) sensitivity = 0.1f;
        
        PlayerPrefs.SetFloat("MouseSensitivityY", sensitivity);
        
        if(CameraController.HasInstance)
            CameraController.Instance.SetSensitivityY(sensitivity);
        
        mouseSensitivityYText.text = sensitivity.ToString("00.0");
    }
    
    public void ShowOptionsPanel()
    {
        optionsPanel.SetActive(true);
        optionsPanel.transform.localScale = Vector3.zero;
        optionsPanel.transform.DOScale(Vector3.one, 0.5f).SetUpdate(true).SetEase(Ease.OutBack);
    }
    
    public void HideOptionsPanel()
    {
        optionsPanel.transform.localScale = Vector3.one;
        optionsPanel.transform.DOScale(Vector3.zero, 0.5f).SetUpdate(true).SetEase(Ease.InBack).OnComplete(() => optionsPanel.SetActive(false));
    }
}
