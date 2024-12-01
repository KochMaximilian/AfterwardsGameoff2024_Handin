using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityUtils;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private InputActionAsset inputActions;
    
    public Vector2 MoveInput => inputActions["Move"].ReadValue<Vector2>();
    public Vector2 LookInput => inputActions["Look"].ReadValue<Vector2>();
    public InputAction LookInputAction => inputActions["Look"];
    public InputAction SpaceBarInput => inputActions["Jump"];
    public InputAction LeftClick => inputActions["Click"];
    public InputAction RightClick => inputActions["RightClick"];
    public InputAction Escape => inputActions["Escape"];
    public InputAction OpenMenu => inputActions["OpenMenu"];
    public InputAction Interact => inputActions["Interact"];
    
    public string InteractKey => inputActions["InteractKey"].controls[0].displayName;
    public string OpenMenuKey => inputActions["OpenMenuKey"].controls[0].displayName;
    
    
    protected override void Awake()
    {
        base.Awake();
        inputActions.Enable();
    }

    public void DisableInput()
    {
        inputActions.Disable();
    }
    
    public void EnableInput()
    {
        inputActions.Enable();
    }
}