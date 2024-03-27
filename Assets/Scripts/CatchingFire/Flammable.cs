using System;
using UnityEngine;
using UnityEngine.Events;

public class Flammable : MonoBehaviour
{
    [HideInInspector] public UnityEvent igniteEvent;
    [HideInInspector] public UnityEvent extinguishEvent;
    
    [SerializeField] private bool _isOnFire = false;
    [SerializeField] private Fire _fire;

    private void Start()
    {
        _fire.extinguishEvent.AddListener(Extinguish);
    }

    /// <summary>
    /// Called when flammable object should ignite.
    /// </summary>
    public void Ignite()
    {
        if(_isOnFire) return;

        _isOnFire = true;
        _fire.Ignite();
        igniteEvent.Invoke();
    }

    /// <summary>
    /// Called when fire was extinguished on the object.
    /// </summary>
    public void Extinguish()
    {
        if(!_isOnFire) return;
        
        _isOnFire = false;
        extinguishEvent.Invoke();
    }

    /// <summary>
    /// Return true, if flammable object is on fire, otherwise false.
    /// </summary>
    /// <returns></returns>
    public bool IsOnFire()
    {
        return _isOnFire;
    }
}
