using UnityEngine;
using UnityEngine.Events;

public class PhysicsButton : MonoBehaviour
{
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    [SerializeField] private GameObject _button;
    [SerializeField] private AudioSource _clickAudio;
    [SerializeField] private Transform _minButtonPressState;
    [SerializeField] private Transform _maxButtonPressState;

    private bool _isPressed = false;
    private GameObject _presser;

    private void OnCollisionEnter(Collision other)
    {
        if(_isPressed || !other.gameObject.layer.Equals(LayerMask.NameToLayer("Hand")) || Vector3.Angle(other.contacts[0].normal, -transform.up) > 45f) return;

        _button.transform.localPosition = _minButtonPressState.localPosition;
        _presser = other.gameObject;
        Pressed();
    }

    private void OnCollisionExit(Collision other)
    {
        if(!_isPressed && !other.gameObject.Equals(_presser)) return;
        
        _button.transform.localPosition = _maxButtonPressState.localPosition;
        Released();
    }

    private void Pressed()
    {
        _isPressed = true;
        _clickAudio.Play();
        onPressed.Invoke();
    }

    private void Released()
    {
        _isPressed = false;
        onReleased.Invoke();
    }
}
