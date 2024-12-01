using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;

public class ObjectDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject objectToDisplay;
    
    public void DisplayObject()
    {
        objectToDisplay.SetActive(true);
    }
    
    public void HideObject()
    {
        objectToDisplay.SetActive(false);
    }
    
    public void HideObject(float time)
    {
        CoroutineController.Start(HideOrShowObject(time, false));
    }
    
    public void DisplayObject(float time)
    {
        CoroutineController.Start(HideOrShowObject(time, true));
    }
    
    public void DisplayObject(InputActionReference action)
    {
        action.action.performed += DisplayObject;
    }
    
    public void HideObject(InputActionReference action)
    {
        action.action.performed += HideObject;
    }
    
    private void HideObject(InputAction.CallbackContext context)
    {
        HideObject();
        context.action.performed -= HideObject;
    }
    
    private void DisplayObject(InputAction.CallbackContext context)
    {
        DisplayObject();
        context.action.performed -= DisplayObject;
    }
    
    private IEnumerator HideOrShowObject(float time, bool show)
    {
        yield return WaitFor.Seconds(time);
        if (show) DisplayObject();
        else HideObject();
    }
    
   
}