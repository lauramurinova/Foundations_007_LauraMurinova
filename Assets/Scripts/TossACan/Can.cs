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

    /// <summary>
    /// Resets the transform position to initial.
    /// </summary>
    public void ResetToInitial()
    {
        _knockedDown = false;
        _transform.position = _initialPosition;
        _transform.rotation = _initialRotation;
    }

    /// <summary>
    /// Returns true if the can has been knocked down, otherwise false.
    /// </summary>
    public bool IsKnockedDown()
    {
        return _knockedDown;
    }

    private void FixedUpdate()
    {
        if (_knockedDown) return;
        
        //continuously check if the can has been knocked down based on its rotation,
        //knock down threshold can be adjusted by preferences
        float angle = Quaternion.Angle(transform.rotation, _initialRotation);

        if (angle > knockdownThresholdAngle)
        {
            _knockedDown = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //if anything collides with the can, play hit sound
        _hit.Play();
    }
}
