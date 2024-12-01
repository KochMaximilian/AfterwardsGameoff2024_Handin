using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;

public class CameraController : Singleton<CameraController>
{
    [Header("References")]
    [SerializeField] private CinemachinePanTilt panTilt;
    [SerializeField] private CinemachineInputAxisController inputAxisController;
    
    [Header("Settings")]
    [SerializeField] private float sensitivityX = 1f;
    [SerializeField] private float sensitivityY = 1f;

    
    private void Start()
    {
        SetSensitivity(new Vector2(sensitivityX, sensitivityY));
    }
    
    public void SetRotation(Vector2 rotation)
    {
        panTilt.PanAxis.Value = rotation.y;
        panTilt.TiltAxis.Value = rotation.x;
    }
    
    public void SetSensitivity(Vector2 sensitivity)
    {
        sensitivityX = sensitivity.x;
        sensitivityY = sensitivity.y;

        foreach (var controller in inputAxisController.Controllers)
        {
            if (controller.Name == "Look X (Pan)")
            {
                controller.Input.Gain = sensitivityX;

            }
            if (controller.Name == "Look Y (Tilt)")
            {
                controller.Input.Gain = -sensitivityY;
            }            
        }
    }
    
    public void SetSensitivityX(float sensitivity)
    {
        sensitivityX = sensitivity;

        foreach (var controller in inputAxisController.Controllers)
        {
            if (controller.Name == "Look X (Pan)")
            {
                controller.Input.Gain = sensitivityX;

            }
        }
    }
    
    public void SetSensitivityY(float sensitivity)
    {
        sensitivityY = sensitivity;

        foreach (var controller in inputAxisController.Controllers)
        {
            if (controller.Name == "Look Y (Tilt)")
            {
                controller.Input.Gain = -sensitivityY;
            }            
        }
    }
}