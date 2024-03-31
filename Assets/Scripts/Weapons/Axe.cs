using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

public class Axe : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float spinSpeed = 10f;
    [SerializeField] private float _throwThreshold = 0.01f;
    [SerializeField] private XRGrabInteractable _axeInteractable;
    [SerializeField] private AudioSource _throwAudio;
    [SerializeField] private AudioSource _collisionAudio;
    [SerializeField] private ParticleSystem _particleSystem;

    private bool _transitioning = false;
    private ExtendedDirectInteractor _interactor;

    private void Start()
    {
        _axeInteractable.selectEntered.RemoveAllListeners();
        _axeInteractable.selectExited.RemoveAllListeners();
        _axeInteractable.selectEntered.AddListener(AxePickedUp);
        _axeInteractable.selectExited.AddListener(AxeDropped);

        Assert.IsNotNull(_rigidbody, "Rigidbody not assigned to object: " + name);
        Assert.IsNotNull(_particleSystem, "Particle System not assigned to object: " + name);
    }

    public void AxeDropped(SelectExitEventArgs arg0)
    {
        _rigidbody.isKinematic = false;
        
        if(_rigidbody.velocity.magnitude < _throwThreshold) return;

        FreezeRotation(true);
        _transitioning = true;
        _throwAudio.Play();
    }
    
    public void AxePickedUp(SelectEnterEventArgs arg0)
    {
        _transitioning = false;
        _rigidbody.isKinematic = false;
        FreezeRotation(false);

        if (_interactor)
        {
            _interactor.OnDoubleSelect.RemoveAllListeners();
        }

        _interactor = arg0.interactor as ExtendedDirectInteractor;

        if (!_interactor) return;
        
        _interactor.OnDoubleSelect.AddListener(Recall);
    }

    public void Recall()
    {
        if(!_interactor) return;
        
        _transitioning = false;
        _rigidbody.isKinematic = false;
        _interactor.interactionManager.ForceSelect(_interactor, _axeInteractable);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!_transitioning) return;
        
        _transitioning = false;
        _collisionAudio.Play();
        _particleSystem.Play();
        FreezeRotation(false);
        
        Debug.Log(LayerMask.LayerToName(other.gameObject.layer));
        Debug.Log(other.gameObject.name);
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Obstacles")))
        {
            foreach (var contact in other.contacts)
            {
                if (contact.thisCollider.name.Equals("BladeTip"))
                {
                    _rigidbody.isKinematic = true;
                    return;
                }   
            }
        }
    }

    private void Update()
    {
        if (_transitioning)
        {
            transform.Rotate(Vector3.right, spinSpeed * Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        if (_interactor)
        {
            _interactor.OnDoubleSelect.RemoveAllListeners();
        }
    }
    
    private void FreezeRotation(bool enabled)
    {
        if (enabled)
        {
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;
            _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
        }
    }
}