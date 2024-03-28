using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

namespace Weapons
{
    public class ShockPistol : Gun
    {
        [SerializeField] private Renderer[] _gunRenderers;
        [SerializeField] private Material[] _ammoScreenMaterial;

        protected override void Start()
        {
            var activeAmmoSocket = GetComponentInChildren<XRTagLimitedSocketInteractor>();
            _ammoSocket = activeAmmoSocket;
            base.Start();
            
            Assert.IsNotNull(_gunRenderers, "You have not assigned a renderer to gun " + name);
            Assert.IsNotNull(_ammoScreenMaterial, "You have not assigned ammo materials to gun " + name);
        }

        protected override void AmmoAttached(SelectEnterEventArgs arg0)
        {
            base.AmmoAttached(arg0);
            UpdateShockPistolScreen();
        }

        protected override void AmmoDetached(SelectExitEventArgs arg0)
        {
            base.AmmoDetached(arg0);
            UpdateShockPistolScreen();
        }
        
        protected override void Fire(ActivateEventArgs arg0)
        {
            if(!CanFire()) return;
            
            base.Fire(arg0);
            UpdateShockPistolScreen();

            var bullet = Instantiate(_ammoClip.bulletObject, _gunBarrel.position, Quaternion.identity)
                .GetComponent<Rigidbody>();
            
            bullet.AddForce(_gunBarrel.forward * _ammoClip.bulletSpeed, ForceMode.Impulse);
            Destroy(bullet.gameObject, 6f);
        }

        private void UpdateShockPistolScreen()
        {
            if (!_ammoClip)
            {
                AssignScreenMaterial(_ammoScreenMaterial[0]);
                return;
            }
            
            AssignScreenMaterial(_ammoScreenMaterial[_ammoClip.amount]);
        }

        private void AssignScreenMaterial(Material newMaterial)
        {
            foreach (var rend in _gunRenderers)
            {
                if (!rend.gameObject.activeSelf) return;
                
                var mats = rend.materials;
                mats[1] = newMaterial;
                rend.materials = mats;
            }
        }
    }
}
