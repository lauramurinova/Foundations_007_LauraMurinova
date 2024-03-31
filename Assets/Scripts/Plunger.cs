using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

public class Plunger : MonoBehaviour
{
    enum PlungerType
    {
        Pull = 0,
        Push = 1
    }
    
    public bool active = false;

    public GameObject _plunger;
    
    [SerializeField] private float _force = 10f;
    [SerializeField] private float _radius = 13f;
    [SerializeField] private PlungerType _type = PlungerType.Pull;
    [SerializeField] private LayerMask _plungerInteractionLayers;

    private Vector3 direction;
    public void Enable(bool enabled)
    {
        active = enabled;
    }

    private void FixedUpdate()
    {
        if(!active) return;
        
        ManipulateRigidbodies(Physics.OverlapSphere(transform.position, _radius, _plungerInteractionLayers));
    }

    private void ManipulateRigidbodies(Collider[] colliders)
    {
        foreach (var collider in colliders)
        {
            if(!collider.TryGetComponent(out Rigidbody rigidbody)) continue;

            rigidbody.velocity *= 0.8f;
            if (IsAnotherPlungerInAreaActive())
            {
               //TODO
            }
            else if (_type.Equals(PlungerType.Push))
            {
                direction = (transform.position - collider.transform.position);
                rigidbody.AddForce(-direction.normalized * (_force), ForceMode.VelocityChange);
            }
            else if(_type.Equals(PlungerType.Pull))
            {
                direction = (transform.position - collider.transform.position);
                rigidbody.AddForce(direction.normalized * (_force), ForceMode.VelocityChange);
            }
        }
    }

    private bool IsAnotherPlungerInAreaActive()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out Plunger plunger) && plunger != this)
            {
                if (plunger.active)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, _plunger.transform.position);
    }
}