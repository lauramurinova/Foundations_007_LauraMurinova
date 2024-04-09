using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

public class ControlVolume : XRBaseInteractable
{
    [SerializeField] private Rigidbody _interactable;
    [SerializeField] private Transform _rotationReference;
    [SerializeField] private float _movementForce = 5f;
    [SerializeField] private float _rotationSpeed = 5f;
    
    [SerializeField] private Color _disabledColor;
    [SerializeField] private Color _enabledColor;

    private bool _active = false;
    private Rigidbody _interactor;
    private MeshRenderer _renderer;
    private Vector3 _previousHandPosition;

    private void Start()
    {
        Assert.IsNotNull(_interactable, "Have not assigned interactable on Control Volume " + name);
        Assert.IsNotNull(_rotationReference, "Have not assigned rotation reference on Control Volume " + name);

        _renderer = GetComponent<MeshRenderer>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        // activate the control volume
        EnableInteractable();
        _rotationReference.SetParent(_interactor.transform);
        _active = true;
        _previousHandPosition = _interactor.transform.position;
        _renderer.material.color = _enabledColor;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // deactivate the control volume
        _active = false;
        _renderer.material.color = _disabledColor;
        DisableInteractable();
        _rotationReference.SetParent(transform);
    }
    private void FixedUpdate()
    {
        if(!_interactor || !_active) return;

        HandleRotation();
        HandleMovement();
    }

    /// <summary>
    /// Handles smooth movement of the assigned interactable based on the interactor movement in the volume.
    /// </summary>
    private void HandleMovement()
    {
        Vector3 handMovement = _interactor.transform.position - _previousHandPosition;
        _interactable.velocity += handMovement * (_movementForce + (handMovement.magnitude*10));
        _previousHandPosition = _interactor.transform.position;
    }

    /// <summary>
    /// Handles smooth rotation of the assigned interactable based on the rotation reference.
    /// </summary>
    private void HandleRotation()
    {
        var targetRotation = Quaternion.LookRotation(_rotationReference.forward, _rotationReference.up);
        _interactable.rotation = Quaternion.Slerp(_interactable.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Hand")))
        {
            if (_interactor == null)
            {
                // save interactor (hand) reference
                _interactor = other.attachedRigidbody;
                
                // if its active (user is still pressing the button), activate the volume interactions
                if (_active)
                {
                    _renderer.material.color = _enabledColor;
                    EnableInteractable();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if the interactor (hand) left the volume, disable interactions
        if (other.attachedRigidbody == _interactor)
        {
            _interactor = null;
            _renderer.material.color = _disabledColor;
            DisableInteractable();
        }
    }
    
    /// <summary>
    /// Enables physics on the assigned interactable.
    /// </summary>
    private void EnableInteractable()
    {
        _interactable.isKinematic = false;
        _interactable.useGravity = false;
    }

    /// <summary>
    /// Disables physics on the assigned interactable.
    /// </summary>
    private void DisableInteractable()
    {
        _interactable.isKinematic = true;
        _interactable.useGravity = true;
    }
}
