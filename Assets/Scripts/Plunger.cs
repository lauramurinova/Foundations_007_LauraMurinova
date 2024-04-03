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

    /// <summary>
    /// Sets the plunger active/inactive.
    /// </summary>
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

    /// <summary>
    /// Returns true if plunger is active (physics are applied), false otherwise.
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return _active;
    }

    /// <summary>
    /// Sets the plunger material to the specified one.
    /// </summary>
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

    /// <summary>
    /// Called when plunger is active. Based on the conditions (type of plunger, other plungers in area) it manipulates rigidbodies inside the defined area.
    /// </summary>
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
                ManipulateRigidbodiesWithNoOtherPlunger(rigidbody);
            }
        }
    }

    /// <summary>
    /// Called when no other plunger is active within the area.
    /// Either pulls or pushes away rigidbodies, based on the plunger type.
    /// </summary>
    private void ManipulateRigidbodiesWithNoOtherPlunger(Rigidbody rigidbody)
    {
        if (_type.Equals(PlungerType.Pull))
        {
            rigidbody.velocity *= 0.5f;
            PullRigidbody(rigidbody);
        }
        else if (_type.Equals(PlungerType.Push))
        {
            rigidbody.velocity *= 0.85f;
            PushRigidbody(rigidbody);
        }
    }

    /// <summary>
    /// Called when another plunger is active in the area.
    /// Keeps the rigidbody floating in front of the plungers.
    /// </summary>
    private void ManipulateRigidbodiesIfAnotherPlungerActive(Rigidbody rigidbody, Plunger plunger)
    {
        rigidbody.velocity *= 0.5f;
        KeepRigidbodyInPlace(rigidbody, plunger);
    }

    /// <summary>
    /// Pushes the rigidbody away from the plunger.
    /// Applies negative force towards the rigidbody.
    /// </summary>
    private void PushRigidbody(Rigidbody rigidbody)
    {
        var direction = (rigidbody.transform.position - _plungerForcePoint.position);
        rigidbody.AddForce(direction.normalized * (_force), ForceMode.Acceleration);
    }

    /// <summary>
    /// Pulls the rigidbody towards the plunger.
    /// </summary>
    private void PullRigidbody(Rigidbody rigidbody)
    {
        var direction = _plungerForcePoint.position - rigidbody.transform.position;
        var distance = Vector3.Distance(_plungerForcePoint.position ,rigidbody.transform.position);
        rigidbody.AddForce(direction.normalized * (_force * distance), ForceMode.Acceleration);
    }

    /// <summary>
    /// Called when another plunger is active within the area.
    /// Keeps the rigidbody floating in front of both plungers.
    /// </summary>
    private void KeepRigidbodyInPlace(Rigidbody rigidbody, Plunger plunger)
    {
        var newPosition = ((_plungerForcePoint.position + plunger._plungerForcePoint.position) / 2f) + _plungerForcePoint.right + _plungerForcePoint.right;
        var direction = (newPosition - rigidbody.transform.position);
        var distance = Vector3.Distance(newPosition ,rigidbody.transform.position);
        rigidbody.AddForce(direction * (_force * distance * 0.25f), ForceMode.Acceleration);
    }

    /// <summary>
    /// Returns true if another plunger is active inside the radius of this plunger, false otherwise.
    /// Returns also the plunger if it was found.
    /// </summary>
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