using System;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsButton : MonoBehaviour
{
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    [SerializeField] private float _threshold = 0.1f;
    [SerializeField] private float _deadzone = 0.025f;

    private bool _isPressed = false;
    private Vector3 _startPos;
    private ConfigurableJoint _joint;

    private void Start()
    {
        _startPos = transform.localPosition;
        _joint = GetComponent<ConfigurableJoint>();
    }

    private void Update()
    {
        if (!_isPressed && GetValue() + _threshold >= 1)
        {
            Pressed();
        }

        if (_isPressed && GetValue() - _threshold <= 0)
        {
            Released();
        }
    }

    private float GetValue()
    {
        
        var value = Vector3.Distance(_startPos, transform.localPosition) / _joint.linearLimit.limit;
        if (Math.Abs(value) < _deadzone)
        {
            value = 0;
        }

        return Mathf.Clamp(value, -1f, 1f);
        
    }

    private void Pressed()
    {
        _isPressed = true;
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
