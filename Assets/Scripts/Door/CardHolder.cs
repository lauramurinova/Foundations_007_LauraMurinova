using TMPro;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    [SerializeField] private Animator _doorAnimation;
    [SerializeField] private AccessCard.AccessGroup _accessGroup;
    [SerializeField] private Transform _attachPoint;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _greenMaterial;
    [SerializeField] private Material _redMaterial;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private AudioSource _accesAudioSource;
    [SerializeField] private AudioSource _doorAudioSource;
    [SerializeField] private AudioClip _accessGrantedClip;
    [SerializeField] private AudioClip _accesDeniedClip;
    [SerializeField] private Light _alarmLight;

    private float _accessTimer = 0f;
    private float _accessTime = 3f;
    private bool _accessGranted = false;
    private bool _accessDenied = false;

    /// <summary>
    /// Checks whether a card was held near the holder.
    /// If the card has the right access group grant access otherwise deny access. 
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent(out AccessCard accessCard)) return;

        if (_accessGranted) return;
        
        if (accessCard._accessCardGroup == _accessGroup)
        {
            GrantAccess();
        }
        else 
        {
            DenyAccess();
        }
    }

    /// <summary>
    /// Sets the holder into denied access state. Sets the light, text and plays audio clip.
    /// </summary>
    private void DenyAccess()
    {
        _accessTimer = 0f;
        _accessDenied = true;
        _accesAudioSource.clip = _accesDeniedClip;
        _accesAudioSource.Play();
        _text.text = "Permission Denied";
        _text.color = Color.red;
        _alarmLight.color = Color.red;
    }

    /// <summary>
    /// Sets the holder into granted access state. Sets the light, text and plays audio clip.
    /// </summary>
    private void GrantAccess()
    {
        _accessTimer = 0f;
        _accessGranted = true;
        _accesAudioSource.clip = _accessGrantedClip;
        _accesAudioSource.Play();
        OpenDoor();
        Material[] materials = _renderer.materials;
        materials[0] = _greenMaterial;
        _renderer.materials = materials;
        _text.text = "Permission Granted";
        _text.color = Color.green;
        _alarmLight.color = Color.green;
    }

    private void Update()
    {
        if (_accessGranted || _accessDenied)
        {
            _accessTimer += Time.deltaTime;
        }

        // after set time reset the holder to neutral state
        if (_accessTimer > _accessTime)
        {
            Reset();
        }
    }

    /// <summary>
    /// Resets the holder. Sets the light and deletes shown text.
    /// </summary>
    private void Reset()
    {
        if (_accessGranted)
        {
            CloseDoor();
            _alarmLight.color = Color.red;
        }

        _accessGranted = false;
        _accessDenied = false;
        _accessTimer = 0f;
        Material[] materials = _renderer.materials;
        materials[0] = _redMaterial;
        _renderer.materials = materials;
        _text.text = "";
        _text.color = Color.white;
    }

    /// <summary>
    /// Opens the door associated with this card holder.
    /// </summary>
    public void OpenDoor()
    {
        _doorAnimation.SetBool("Open", true);
        _doorAnimation.SetBool("Close", false);
        _doorAudioSource.Play();
    }
    
    /// <summary>
    /// Closes the door associated with this card holder.
    /// </summary>
    public void CloseDoor()
    {
        _doorAnimation.SetBool("Close", true);
        _doorAnimation.SetBool("Open", false);
        _doorAudioSource.Play();
    }
}
