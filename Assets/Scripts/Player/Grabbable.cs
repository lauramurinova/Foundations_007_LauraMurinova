using UnityEngine;
using UnityEngine.Events;

public class Grabbable : MonoBehaviour
{
    [HideInInspector] public UnityEvent throwEvent = new UnityEvent();
    [HideInInspector] public UnityEvent grabEvent = new UnityEvent();
    [HideInInspector] public UnityEvent releaseEvent = new UnityEvent();
    public Transform grabPoint;
    
    /// <summary>
    /// Resets transform position and rotation to default values.
    /// </summary>
    public void ResetTransform()
    {
        Transform objectTransform = transform;
        objectTransform.localPosition = Vector3.zero;
        objectTransform.localRotation = Quaternion.identity;
        
        if (grabPoint)
        {
            objectTransform.position = grabPoint.position;
            objectTransform.rotation = grabPoint.rotation;
        }
    }
}
