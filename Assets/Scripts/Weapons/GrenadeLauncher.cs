using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;
using Weapons;

public class GrenadeLauncher : Gun
{
    [SerializeField] private Projection _projection;
    [SerializeField] private GrenadeBall _bullet;
    [SerializeField] private Material _ammoDisabledMat;

    protected override void Start()
    {
        var activeAmmoSocket = GetComponentInChildren<XRTagLimitedSocketInteractor>();
        _ammoSocket = activeAmmoSocket;
        
        base.Start();
            
        Assert.IsNotNull(_bullet, "You have not assigned a bullet to gun " + name);
        Assert.IsNotNull(_projection, "You have not assigned projection script to gun " + name);
    }
    
    private void Update()
    {
        _projection.SimulateTrajectory(_bullet, _gunBarrel.position, _gunBarrel.forward * 6f);
    }

    protected override void Fire(ActivateEventArgs arg0)
    {
        if(!CanFire()) return;
        
        base.Fire(arg0);

        var bullet = Instantiate(_bullet, _gunBarrel.position, Quaternion.identity);
        bullet.Init(_gunBarrel.forward * 6f, false);

        if (!CanFire())
        {
            _ammoClip.GetComponentInChildren<Renderer>().material = _ammoDisabledMat;
            _projection.GetComponent<LineRenderer>().enabled = false;
        }
    }
}