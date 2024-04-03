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

    /// <summary>
    /// Called when axe was released, if it was thrown with enough strength, enable the axe to get stuck in objects.
    /// Ensures axe like transitions.
    /// </summary>
    public void AxeDropped(SelectExitEventArgs arg0)
    {
        _rigidbody.isKinematic = false;
        
        if(_rigidbody.velocity.magnitude < _throwThreshold) return;

        FreezeRotation(true);
        _transitioning = true;
        _throwAudio.Play();
    }
    
    /// <summary>
    /// Called when axe is picked up, makes it ready (physics) to be thrown to objects.
    /// </summary>
    public void AxePickedUp(SelectEnterEventArgs arg0)
    {
        transform.parent = null;
        _transitioning = false;
        _rigidbody.isKinematic = false;
        FreezeRotation(false);

        SetRecallListener(arg0);
    }

    /// <summary>
    /// Set listener for axe recall - on double select (double trigger).
    /// </summary>
    private void SetRecallListener(SelectEnterEventArgs arg0)
    {
        if (_interactor)
        {
            _interactor.OnDoubleSelect.RemoveAllListeners();
        }

        _interactor = arg0.interactor as ExtendedDirectInteractor;

        if (!_interactor) return;
        
        _interactor.OnDoubleSelect.AddListener(Recall);
    }

    /// <summary>
    /// Gets the axe object to last used players interactor. 
    /// </summary>
    public void Recall()
    {
        if(!_interactor) return;

        transform.parent = null;
        _transitioning = false;
        _rigidbody.isKinematic = false;
        _interactor.interactionManager.ForceSelect(_interactor, _axeInteractable);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(!_transitioning) return;
        
        _transitioning = false;
        FreezeRotation(false);

        EnemyController enemyController = null;
        
        // checks whether it collided with a wall or enemy and if the axe hit it with the right part (thanks to angles)
        if ((other.gameObject.layer.Equals(LayerMask.NameToLayer("Obstacles")) || other.gameObject.TryGetComponent(out enemyController)) && Vector3.Angle(other.contacts[0].normal, -transform.up) < 45f)
        {
            foreach (var contact in other.contacts)
            {
                if (contact.thisCollider.name.Equals("Blade"))
                {
                    if (enemyController)
                    {
                        enemyController.SetStunned();
                    }
                    
                    SetAxeStuck(other);
                    return;
                }   
            }
        }
    }

    /// <summary>
    /// Sets axe into stuck state, without physics.
    /// </summary>
    private void SetAxeStuck(Collision other)
    {
        transform.parent = other.transform;
        _collisionAudio.Play();
        _particleSystem.Play();
        _rigidbody.isKinematic = true;
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
    
    /// <summary>
    /// Called when axe is thrown, enables nice axe rotation.
    /// </summary>
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