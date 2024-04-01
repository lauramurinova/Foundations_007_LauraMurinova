using System;
using UnityEngine;

public class Plunger : MonoBehaviour
{
    enum PlungerType
    {
        Pull = 0,
        Push = 1
    }
    
    public bool active = false;

    public GameObject _plunger;
    public GameObject _sphere;
    
    [SerializeField] private float _force = 10f;
    [SerializeField] private float _radius = 13f;
    [SerializeField] private PlungerType _type = PlungerType.Pull;
    [SerializeField] private LayerMask _plungerInteractionLayers;

    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _disabledMat;
    [SerializeField] private Material _enabledMat;

    private Vector3 direction;
    public void Enable(bool enabled)
    {
        if (enabled)
        {
            SetActiveMaterial(_renderer, _enabledMat);
        }
        else
        {
            SetActiveMaterial(_renderer, _disabledMat);
        }
        active = enabled;
    }

    private void SetActiveMaterial(MeshRenderer renderer, Material material)
    {
        var materials = renderer.materials;
        materials[1] = material;
        renderer.materials = materials;
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

            rigidbody.velocity *= 0.5f;
            rigidbody.useGravity = false;
            if (IsAnotherPlungerInAreaActive())
            {
                // rigidbody.velocity *= 0f;
                // Vector3 newPosition = ((transform.position + _plunger.transform.position) / 2f) + transform.forward;
                // direction = newPosition - collider.transform.position;
                // rigidbody.transform.position = newPosition;
                
                if (_type.Equals(PlungerType.Pull))
                {
                    Vector3 newPosition = ((transform.position + _plunger.transform.position) / 2f) + transform.forward * 3f;
                    direction = newPosition - collider.transform.position;
                    rigidbody.AddForce(direction.normalized * (_force), ForceMode.Acceleration);
                }
                else if (_type.Equals(PlungerType.Push))
                {
                    Vector3 newPosition = transform.position + transform.forward * 3f;
                    direction = (newPosition - (transform.position));
                    rigidbody.AddForce(-direction.normalized * (_force), ForceMode.Acceleration);
                }
            }
            else
            {
                rigidbody.useGravity = true;
                if (_type.Equals(PlungerType.Pull))
                {
                    direction = transform.position - collider.transform.position;
                    rigidbody.AddForce(direction.normalized * (_force), ForceMode.Acceleration);
                }
                else if (_type.Equals(PlungerType.Push))
                {
                    direction = (collider.transform.position - transform.position);
                    rigidbody.AddForce(direction.normalized * (_force), ForceMode.Acceleration);
                }
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
        
        Vector3 newPosition = ((transform.position + _plunger.transform.position) / 2f) + transform.forward;
        Gizmos.DrawLine(transform.position, newPosition);
    }
}