using UnityEngine;
using UnityEngine.Events;

public class Fire : MonoBehaviour
{
    [HideInInspector] public UnityEvent extinguishEvent;
    
    [SerializeField] private GameObject _fire;
    [SerializeField] private GameObject _smoke;
    [SerializeField] private LayerMask _flammableLayers;
    [SerializeField] private LayerMask _waterLayer;
    
    [Header("Fire Spread Values")]
    [SerializeField] private float _ignitionRadius = 0.5f;
    [SerializeField] private Transform _spredPoint1;
    [SerializeField] private Transform _spredPoint2;

    private float _ignationTime = 2f;
    private float _ignationTimer = 0f;
    
    private float _extinguishTime = 0.5f;
    private float _extinguishTimer = 0f;

    private void Update()
    {
        CheckFlammableObjectsNearby();
        CheckForExtinguish();
    }

    /// <summary>
    /// Checks whether the fire collided/is in water, if yes, after the set time, extinguish the fire.
    /// </summary>
    private void CheckForExtinguish()
    {
        if (!_fire.activeSelf || !IsInWater())
        {
            _extinguishTimer = 0f;
            return;
        };

        _extinguishTimer += Time.deltaTime;

        if (_extinguishTimer > _extinguishTime)
        {
            _extinguishTimer = 0f;
            Extinguish();
            extinguishEvent.Invoke();
        }
    }

    /// <summary>
    /// Checks whether flammable objects are nearby, if yes after 2 second wait time ignite them.
    /// </summary>
    private void CheckFlammableObjectsNearby()
    {
        if (!_fire.activeSelf) return;

        Flammable flammable = FlammableObjectNearby();
        if (!flammable)
        {
            _ignationTimer = 0f;
            return;
        };
        
        _ignationTimer += Time.deltaTime;

        if (_ignationTimer > _ignationTime)
        {
            _ignationTimer = 0f;
            flammable.Ignite();
        }
    }

    /// <summary>
    /// Returns true if the object is in water, false otherwise.
    /// </summary>
    private bool IsInWater()
    {
        foreach (Collider collider in GetObjectsNearby(_waterLayer))
        {
            if (collider.gameObject.layer.Equals(LayerMask.NameToLayer("Water")))
            {
                Debug.Log(gameObject.name + " is in " + collider.gameObject.name);
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Returns flammable object nearby, if found. Otherwise returns null.
    /// </summary>
    private Flammable FlammableObjectNearby()
    {
        foreach (Collider collider in GetObjectsNearby(_flammableLayers))
        {
            if (!collider.TryGetComponent(out Flammable flammable)) continue;
            
            if(flammable.IsOnFire()) continue;

            return flammable;

        }
        return null;
    }

    /// <summary>
    /// Returns colliders of objects of set layer nearby.
    /// </summary>
    private Collider[] GetObjectsNearby(LayerMask layerMask)
    {
        return Physics.OverlapCapsule(_spredPoint1.position, _spredPoint2.position , _ignitionRadius, layerMask);
    }
    
    /// <summary>
    /// Sets fire effect.
    /// </summary>
    public void Ignite()
    {
        _fire.SetActive(true);
        _smoke.SetActive(false);
    }

    /// <summary>
    /// Sets extinguish effect.
    /// </summary>
    public void Extinguish()
    {
        _fire.SetActive(false);
        _smoke.SetActive(true);
        Invoke(nameof(DisableSmoke), 1.5f);
    }

    private void DisableSmoke()
    {
        _smoke.SetActive(false);
    }
}
