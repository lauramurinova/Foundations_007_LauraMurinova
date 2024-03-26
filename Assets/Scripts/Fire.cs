using UnityEngine;
using UnityEngine.Events;

public class Fire : MonoBehaviour
{
    [HideInInspector] public UnityEvent extinguishEvent;
    
    [SerializeField] private GameObject _fire;
    [SerializeField] private GameObject _smoke;
    [SerializeField] private LayerMask _flammableLayers;
    [SerializeField] private float _ignitionRadius = 0.5f;

    private float _ignationTime = 2f;
    private float _ignationTimer = 0f;

    private void Update()
    {
        if(!_fire.activeSelf) return;

        _ignationTimer += Time.deltaTime;

        if (_ignationTimer > _ignationTime)
        {
            _ignationTimer = 0f;
            CheckForFlammableObjects();
        }
    }
    
    private void CheckForFlammableObjects()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _ignitionRadius, _flammableLayers);

        foreach (Collider collider in colliders)
        {
            if(!collider.TryGetComponent(out Flammable flammable) || flammable.IsOnFire()) continue;
            
            flammable.Ignite();
            break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.layer.Equals(LayerMask.NameToLayer("Water")) || !_fire.activeSelf) return;

        Extinguish();
        extinguishEvent.Invoke();
    }
    
    public void Ignite()
    {
        _fire.SetActive(true);
        _smoke.SetActive(false);
    }

    public void Extinguish()
    {
        _fire.SetActive(false);
        _smoke.SetActive(true);
    }
}
