using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ExtendedDirectInteractor : XRDirectInteractor
{
    // Event for when user double selects (triggers) on the controller.
    [Header("Double Events")]
    public UnityEvent OnDoubleSelect;
    
    public void TriggerDoubleSelect()
    {
        OnDoubleSelect?.Invoke();
    }
}
