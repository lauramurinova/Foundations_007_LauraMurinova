using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class ControlVolume : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float rotationSpeed = 10f;
    
    [SerializeField] private Rigidbody _interactable;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private Color _enabledColor;

    private ControlVolumeInput[] _controlVolumeInputs;
    private bool _active = false;
    private Rigidbody _interactor;
    private MeshRenderer _renderer;
    private Vector3 _previousHandPosition;
    private Quaternion _previousRotation;
    private bool _isRotating = false;

    private void Start()
    {
        Assert.IsNotNull(_interactable, "Have not assigned interactable on Control Volume " + name);

        _controlVolumeInputs = GameObject.FindObjectsOfType<ControlVolumeInput>();
        foreach (var controlVolume in _controlVolumeInputs)
        {
            controlVolume.controlVolumePressed.AddListener(Activate);
            controlVolume.controlVolumeReleased.AddListener(Deactivate);
        }

        _renderer = GetComponent<MeshRenderer>();
    }

    public void Deactivate()
    {
        _active = false;
        _interactor = null;
        _renderer.material.color = _disabledColor;
        _interactable.isKinematic = true;
        _interactable.useGravity = true;
        StopAllCoroutines();
        _isRotating = false;
    }

    public void Activate()
    {
        _active = true;
    }

    private void FixedUpdate()
    {
        if(!_interactor) return;
        
         _interactable.rotation = Quaternion.Slerp(_interactable.rotation, _interactor.transform.rotation, rotationSpeed * Time.deltaTime);
            
                Vector3 handMovement = _interactor.transform.position - _previousHandPosition;

            float forwardDot = Vector3.Dot(handMovement.normalized, _interactor.transform.forward);
            float backwardDot = Vector3.Dot(handMovement.normalized, -_interactor.transform.forward);
            float rightDot = Vector3.Dot(handMovement.normalized, _interactor.transform.up);
            float leftDot = Vector3.Dot(handMovement.normalized, -_interactor.transform.up);
            float upDot = Vector3.Dot(handMovement.normalized, _interactor.transform.right);
            float downDot = Vector3.Dot(handMovement.normalized, -_interactor.transform.right);

            if (_interactor.velocity.magnitude > 0.05f)
            {
                if (forwardDot > 0.8f)
                {
                    _interactable.velocity *= 0.9f;
                    _interactable.AddForce(_interactor.transform.forward * (handMovement.magnitude * 800f),
                        ForceMode.Acceleration);
                }
                else if (backwardDot > 0.8f)
                {
                    _interactable.velocity *= 0.9f;
                    _interactable.AddForce(-_interactor.transform.forward * (800f * handMovement.magnitude),
                        ForceMode.Acceleration);
                }
                else if (rightDot > 0.8f)
                {
                    _interactable.velocity *= 0.9f;
                    _interactable.AddForce(_interactor.transform.up * (800f * handMovement.magnitude),
                        ForceMode.Acceleration);
                }
                else if (leftDot > 0.8f)
                {
                    _interactable.velocity *= 0.9f;
                    _interactable.AddForce(-_interactor.transform.up * (800f * handMovement.magnitude),
                        ForceMode.Acceleration);
                }
                else if (upDot > 0.8f)
                {
                    _interactable.velocity *= 0.9f;
                    _interactable.AddForce(_interactor.transform.right * (800f * handMovement.magnitude),
                        ForceMode.Acceleration);
                }
                else if (downDot > 0.8f)
                {
                    _interactable.velocity *= 0.9f;
                    _interactable.AddForce(-_interactor.transform.right * (800f * handMovement.magnitude),
                        ForceMode.Acceleration);
                }
            }
            else
            {
                _interactable.velocity *= 0.55f;
            }
            
            _previousHandPosition = _interactor.transform.position;
            _renderer.material.color = _enabledColor;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Hand")) && _active)
        {

            if (_interactor == null)
            {
                _interactor = other.attachedRigidbody;
                _interactable.isKinematic = false;
                _interactable.useGravity = false;
                _previousHandPosition = _interactor.transform.position;
                _previousRotation = _interactor.transform.rotation;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Deactivate();
    }
}
