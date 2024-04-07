using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControlVolumeInput : MonoBehaviour
{
    public InputActionReference controlVolumeButton;
    public UnityEvent controlVolumePressed;
    public UnityEvent controlVolumeReleased;
    
    private void Start()
    {
        controlVolumeButton.action.started += ControlVolumePressed;
        controlVolumeButton.action.canceled += ControlvolumeReleased;
    }

    private void ControlVolumePressed(InputAction.CallbackContext obj)
    {
        controlVolumePressed.Invoke();
    }
    
    private void ControlvolumeReleased(InputAction.CallbackContext obj)
    {
        controlVolumeReleased.Invoke();
    }

    private void OnDestroy()
    {
        controlVolumeButton.action.started -= ControlVolumePressed;
        controlVolumeButton.action.started -= ControlvolumeReleased;
    }
}
