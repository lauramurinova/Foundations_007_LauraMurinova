using System;
using UnityEngine;
using UnityEngine.Events;

public class EasyPhysicsButton : MonoBehaviour
{
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    [SerializeField] private GameObject _button;
    [SerializeField] private AudioSource _clickAudio;

    private bool _isPressed = false;
    private GameObject _presser;

    private void OnCollisionEnter(Collision other)
    {
        if(_isPressed || !other.gameObject.layer.Equals(LayerMask.NameToLayer("Hand"))) return;

        _button.transform.localPosition = new Vector3(0.092f, 0.0087f, 0);
        _presser = other.gameObject;
        Pressed();
    }

    private void OnCollisionExit(Collision other)
    {
        if(!_isPressed && !other.gameObject.Equals(_presser)) return;
        
        _button.transform.localPosition = new Vector3(0.092f, 0.0211f, 0);
        Released();
    }

    private void Pressed()
    {
        _isPressed = true;
        _clickAudio.Play();
        onPressed.Invoke();
        Debug.Log("PRESSED");
    }

    private void Released()
    {
        _isPressed = false;
        onReleased.Invoke();
        Debug.Log("RELEASED");
    }
}
