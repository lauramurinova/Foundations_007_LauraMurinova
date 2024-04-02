using UnityEngine;

public class Plunger : MonoBehaviour
{
    enum PlungerType
    {
        Pull = 0,
        Push = 1
    }
    
    [SerializeField] public Transform _plungerForcePoint;
    [SerializeField] private float _force = 10f;
    [SerializeField] private float _radius = 13f;
    [SerializeField] private PlungerType _type = PlungerType.Pull;
    [SerializeField] private LayerMask _plungerInteractionLayers;

    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _disabledMat;
    [SerializeField] private Material _enabledMat;

    private bool _active = false;
    private Plunger _anotherPlunger;

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
        _active = enabled;
    }

    public bool IsActive()
    {
        return _active;
    }

    private void SetActiveMaterial(MeshRenderer renderer, Material material)
    {
        var materials = renderer.materials;
        materials[1] = material;
        renderer.materials = materials;
    }

    private void FixedUpdate()
    {
        if(!_active) return;
        
        ManipulateRigidbodies(Physics.OverlapSphere(_plungerForcePoint.position, _radius, _plungerInteractionLayers));
        
    }

    private void ManipulateRigidbodies(Collider[] colliders)
    {
        foreach (var collider in colliders)
        {
            if(!collider.TryGetComponent(out Rigidbody rigidbody)) continue;

            if (IsAnotherPlungerInAreaActive(out Plunger plunger))
            {
                ManipulateRigidbodiesIfAnotherPlungerActive(rigidbody, plunger);
            }
            else
            {
                ManipulateRigidbodiesWithNoOtherPlunger(rigidbody, collider);
            }
        }
    }

    private void ManipulateRigidbodiesWithNoOtherPlunger(Rigidbody rigidbody, Collider collider)
    {
        if (_type.Equals(PlungerType.Pull))
        {
            rigidbody.velocity *= 0.5f;
            PullRigidbody(collider, rigidbody);
        }
        else if (_type.Equals(PlungerType.Push))
        {
            rigidbody.velocity *= 0.85f;
            PushRigidbody(collider, rigidbody);
        }
    }

    private void ManipulateRigidbodiesIfAnotherPlungerActive(Rigidbody rigidbody, Plunger plunger)
    {
        rigidbody.velocity *= 0.5f;
        KeepRigidbodyInPlace(rigidbody, plunger);
    }

    private void PushRigidbody(Collider collider, Rigidbody rigidbody)
    {
        var direction = (collider.transform.position - _plungerForcePoint.position);
        rigidbody.AddForce(direction.normalized * (_force), ForceMode.Acceleration);
    }

    private void PullRigidbody(Collider collider, Rigidbody rigidbody)
    {
        var direction = _plungerForcePoint.position - collider.transform.position;
        var distance = Vector3.Distance(_plungerForcePoint.position ,collider.transform.position);
        rigidbody.AddForce(direction.normalized * (_force * distance), ForceMode.Acceleration);
    }

    private void KeepRigidbodyInPlace(Rigidbody rigidbody, Plunger plunger)
    {
        var newPosition = ((_plungerForcePoint.position + plunger._plungerForcePoint.position) / 2f) + _plungerForcePoint.right + _plungerForcePoint.right;
        var direction = (newPosition - rigidbody.transform.position);
        var distance = Vector3.Distance(newPosition ,rigidbody.transform.position);
        rigidbody.AddForce(direction * (_force * distance * 0.25f), ForceMode.Acceleration);
    }

    private bool IsAnotherPlungerInAreaActive(out Plunger plungerFound)
    {
        Collider[] colliders = Physics.OverlapSphere(_plungerForcePoint.position, _radius);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out Plunger plunger) && plunger != this)
            {
                if (plunger.IsActive())
                {
                    plungerFound = plunger;
                    return true;
                }
            }
        }

        plungerFound = null;
        return false;
    }
}