using UnityEngine;
using UnityEngine.Events;

public class Flammable : MonoBehaviour
{
    public UnityEvent igniteEvent;
    public UnityEvent extinguishEvent;
    
    [SerializeField] private bool _isOnFire = false;
    [SerializeField] private Fire _fire;

    public void Ignite()
    {
        if(_isOnFire) return;

        _isOnFire = true;
        _fire.Ignite();
        _fire.extinguishEvent.AddListener(Extinguish);
        igniteEvent.Invoke();
    }

    public void Extinguish()
    {
        if(!_isOnFire) return;
        
        _isOnFire = false;
        _fire.extinguishEvent.RemoveAllListeners();
        extinguishEvent.Invoke();
    }

    public bool IsOnFire()
    {
        return _isOnFire;
    }
}
