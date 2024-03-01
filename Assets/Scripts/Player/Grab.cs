using System;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosition;
    [SerializeField] private Transform _holdPosition;
    [SerializeField] private float _grabRange = 2;
    [SerializeField] private float _throwForce = 20f;
    [SerializeField] private float _snapSpeed = 40f;

    private Rigidbody _grabbedObject;
    private bool _grabPressed = false;

    private void FixedUpdate()
    {
        if (_grabbedObject)
        {
            _grabbedObject.velocity = (_holdPosition.position - _grabbedObject.transform.position) * _snapSpeed;
            
        }
    }

    private void OnGrab()
    {
        if (_grabPressed)
        {
            _grabPressed = false;

            if (!_grabbedObject) return;

            DropGrabbedObject();
        }
        else
        {
            _grabPressed = true;

            if (Physics.Raycast(_cameraPosition.position, _cameraPosition.forward, out RaycastHit hit, _grabRange))
            {
                if (!hit.transform.gameObject.TryGetComponent(out Grabbable grabbable)) return;

                GrabObject(hit.transform.GetComponent<Rigidbody>(), grabbable);
            }
        }
    }

    private void GrabObject(Rigidbody grabbedObject, Grabbable grabbable)
    {
        _grabbedObject = grabbedObject;
        _grabbedObject.transform.parent = _holdPosition;
        _grabbedObject.freezeRotation = true;
        grabbable.ResetTransform();
        grabbable.grabEvent.Invoke();
    }

    private void DropGrabbedObject()
    {
        _grabbedObject.freezeRotation = false;
        _grabbedObject.transform.parent = null;
        _grabbedObject = null;
    }

    private void OnThrow()
    {
        if(!_grabbedObject) return;

        var grabbable = _grabbedObject.GetComponent<Grabbable>();
        _grabbedObject.AddForce(_cameraPosition.forward * _throwForce, ForceMode.Impulse);
        DropGrabbedObject();
        grabbable.throwEvent.Invoke();
    }
}
