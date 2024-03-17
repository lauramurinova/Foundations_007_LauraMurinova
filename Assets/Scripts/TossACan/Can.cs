using System;
using UnityEngine;

public class Can : MonoBehaviour
{
    [SerializeField] private AudioSource _hit;
    [SerializeField] private float knockdownThresholdAngle = 45f;
    
    private bool _knockedDown = false;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Transform _transform;

    private void Start()
    {
        _transform = transform;
        _initialPosition = _transform.position;
        _initialRotation = _transform.rotation;
    }

    public void ResetToInitial()
    {
        _knockedDown = false;
        _transform.position = _initialPosition;
        _transform.rotation = _initialRotation;
    }

    public bool IsKnockedDown()
    {
        return _knockedDown;
    }

    private void FixedUpdate()
    {
        float angle = Quaternion.Angle(transform.rotation, _initialRotation);

        if (angle > knockdownThresholdAngle && !_knockedDown)
        {
            _knockedDown = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        _hit.Play();
    }
}
