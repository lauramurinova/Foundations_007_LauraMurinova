using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class ExtendedDirectInteractor : XRDirectInteractor
{
    [Header("Double Events")]
    public UnityEvent OnDoubleSelect;
    
    public void TriggerDoubleSelect()
    {
        OnDoubleSelect?.Invoke();
    }
}
