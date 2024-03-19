using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunWithoutAmmo : Gun
{
    [SerializeField] private TextMeshPro _ammoLeftText;
    [SerializeField] private AudioClip _fireAmmoClip;
    [SerializeField] private AudioClip _insertAmmoClip;
    [SerializeField] private AudioClip _takeOutAmmoClip;
    
    private Ammo _ammo = null;

    /// <summary>
    /// Fires bullet if has ammo and bullets.
    /// </summary>
    public void FireBullet()
    {
        if(!HasAmmo()) return;

        if(HasBullets()) return;
        
        _audioSource.clip = _fireAmmoClip;
        base.FireBullet();
        _ammo.bullets--;
        _ammoLeftText.text = "Ammo Left:\n " + _ammo.bullets;
    }
    
    /// <summary>
    /// Called on Socket Enter, inserts ammo to the gun.
    /// Also sets ammo text and physics layer with sound effect.
    /// </summary>
    public void InsertAmmo(SelectEnterEventArgs args)
    {
        Debug.Log(args.interactable);
        if (!args.interactable.TryGetComponent(out Ammo ammo)) return;

        _audioSource.clip = _insertAmmoClip;
        _audioSource.Play();
        _ammo = ammo;
        _ammo.gameObject.layer = LayerMask.NameToLayer("Ammo");
        Debug.Log(_ammo.gameObject.layer);
        _ammoLeftText.text = "Ammo Left:\n " + _ammo.bullets;
    }

    /// <summary>
    /// Called on Socket Exit, takes out the ammo from the gun.
    /// Resets ammo text and physics layer with sound effect.
    /// </summary>
    public void TakeOutAmmo(SelectExitEventArgs args)
    {
        if (!args.interactable.TryGetComponent(out Ammo ammo)) return;
        
        _audioSource.clip = _takeOutAmmoClip;
        _audioSource.Play();
        ammo.gameObject.layer = LayerMask.NameToLayer("Grab");
        _ammoLeftText.text = "";
        _ammo = null;
    }

    private bool HasAmmo()
    {
        if (_ammo == null)
        { 
            _ammoLeftText.text = "Insert Ammo";
            return false;
        }
        return true;
    }
    
    private bool HasBullets()
    {
        if (_ammo.bullets == 0)
        {
            _ammoLeftText.text = "No Bullets Left";
            return false;
        }

        return true;
    }
}
