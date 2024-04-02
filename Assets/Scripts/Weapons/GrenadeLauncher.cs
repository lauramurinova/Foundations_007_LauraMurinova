using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Weapons;

public class GrenadeLauncher : Gun
{
    [SerializeField] private Projection _projection;
    
    [SerializeField] private GrenadeBall _bullet;

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
    }
}