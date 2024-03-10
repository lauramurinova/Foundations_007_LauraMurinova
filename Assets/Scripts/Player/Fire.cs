using UnityEngine;
using UnityEngine.Events;

public class Fire : MonoBehaviour
{
    public UnityEvent fireEvent;
    
    /// <summary>
    /// Checks for user input - space for firing.
    /// </summary>
    private void OnFire()
    {
        fireEvent.Invoke();
    }
}
