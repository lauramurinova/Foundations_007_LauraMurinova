using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableGrabbable : Grabbable
{
    [SerializeField] private float _transitionPositionTime = 0.5f;
    
    private Transform _transform;
    private Rigidbody _rigidBody;
    private LayerMask _originalLayer;
    private Coroutine _transition;

    private void Start()
    {
        _transform = transform;
        _rigidBody = GetComponent<Rigidbody>();
        
        //set listeners for grab and release to put on and off the object
        grabEvent.AddListener(PutOn);
        releaseEvent.AddListener(PutOff);
    }

    /// <summary>
    /// Triggers the transition, that makes it look like the player is putting the object on.
    /// Checks for grabPoint, if none exists, create a default one.
    /// </summary>
    private void PutOn()
    {
        if (!grabPoint)
        {
            grabPoint.position = Vector3.zero;
            grabPoint.rotation = Quaternion.identity;
        }
        
        if (_transition == null)
        {
            StartCoroutine(PutOnTransition());
        }
    }
    
    /// <summary>
    /// Triggers the transition of taking off the object.
    /// </summary>
    private void PutOff()
    {
        if (_transition == null)
        {
            StartCoroutine(PutOffTransition());
        }
    }
    
    /// <summary>
    /// Triggers the putting on transition and sets the proper transform and rigidBody settings.
    /// </summary>
    private IEnumerator PutOnTransition()
    {
        _originalLayer = _transform.gameObject.layer;
        _transform.gameObject.layer = LayerMask.NameToLayer("Hideable");
        _rigidBody.isKinematic = true;
        _transform.localRotation = grabPoint.localRotation;
        
        yield return StartCoroutine(TransitionPosition(grabPoint.localPosition + Vector3.up, grabPoint.localPosition, _transitionPositionTime));
        _transition = null;
    }
    
    /// <summary>
    /// Triggers the putting off transition and sets/resets the transform and rigidBody settings.
    /// </summary>
    private IEnumerator PutOffTransition()
    {
        var objectPosition = _transform.position;
        yield return StartCoroutine(TransitionPosition(objectPosition, objectPosition + Vector3.up, _transitionPositionTime));

        _transform.position = FindSafePosition(grabPoint.position);
        _rigidBody.isKinematic = false;
        _transform.gameObject.layer = _originalLayer;
        _transition = null;
    }

    /// <summary>
    /// Ensures smooth transition between two positions during set duration time.
    /// </summary>
    private IEnumerator TransitionPosition(Vector3 originalPosition, Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            _transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _transform.localPosition = targetPosition;
    }

    /// <summary>
    /// Return a safe position near the desired position, where to put the object when taken off (so it doesn't fly away).
    /// </summary>
    private Vector3 FindSafePosition(Vector3 desiredPosition)
    {
        Vector3 safePosition = desiredPosition;
        safePosition += Vector3.up * 0.1f; 
        return safePosition;
    }
}
