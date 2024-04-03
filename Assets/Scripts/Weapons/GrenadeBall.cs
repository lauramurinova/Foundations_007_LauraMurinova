using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GrenadeBall : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _force = 100f;
    [SerializeField] private float _forceRadius = 5f;
    [SerializeField] private float _explodeTime = 0.5f;
    [SerializeField] private LayerMask _interactableLayerMask;
    [SerializeField] private ParticleSystem _bullet;
    [SerializeField] private AudioSource _bulletAudio;
    [SerializeField] private ParticleSystem _blackHole;
    [SerializeField] private AudioSource _blackHoleAudio;

    private bool _explode = false;
    private float _explodeTimer = 0;
    private float _lifeTime = 7;
    private float _lifeTimer = 0;
    private List<Rigidbody> _suckedInRigidbodies = new List<Rigidbody>();
    private bool _isGhost = false;
    private bool _movedObjects = false;
    
    public void Init(Vector3 velocity, bool isGhost)
    {
        _bulletAudio.Play();
        _blackHole.Stop();
        _isGhost = isGhost;
        _rigidbody.AddForce(velocity, ForceMode.Impulse);
    }

    private void Update()
    {
        if(_isGhost) return;
        
        UpdateLifetime();
        
        if(!_explode) return;

        UpdateExplosion();
    }

    private void UpdateLifetime()
    {
        _lifeTimer += Time.deltaTime;

        if (_lifeTimer > _lifeTime)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Takes care of the explosion sequence, first sucking in the rigidbodies and then teleporting them, all within a specified amount of time.
    /// Calls visual effects of the explosion and teleport.
    /// </summary>
    private void UpdateExplosion()
    {
        _explodeTimer += Time.deltaTime;
        if (_explodeTimer < _explodeTime/2)
        {
            SuckInRigidbodies(_suckedInRigidbodies);
            if (!_blackHole.isPlaying)
            {
                ShowBlackHole(transform.position);
            }
        }
        
        if (_explodeTimer > _explodeTime && !_movedObjects)
        {
            TeleportRigidbodiesToRandomPosition(_suckedInRigidbodies);
        }
    }

    /// <summary>
    /// Enables the black hole (that sucks in the objects).
    /// </summary>
    private void ShowBlackHole(Vector3 position)
    {
        _blackHole.transform.position = position;
        _blackHole.Play();
        _blackHoleAudio.Play();
    }

    /// <summary>
    /// Teleports given rigidbodies to a random position in a world.
    /// For testing purposes the random position is disabled and it teleports the object to right.
    /// </summary>
    private void TeleportRigidbodiesToRandomPosition(List<Rigidbody> rigidbodies)
    {
        _movedObjects = true;
        StartCoroutine(MoveRigidbodies(rigidbodies));
        Destroy(gameObject, _explodeTime*2);
    }

    private IEnumerator MoveRigidbodies(List<Rigidbody> rigidbodies)
    {
        // Vector3 randomPosition = new Vector3(Random.Range(15f, 35f), -2f, Random.Range(-20f, -30f)) + Vector3.up;
        //testing purposes
        var randomPosition = _blackHole.transform.position + Vector3.right + Vector3.up;
        
       ShowBlackHole(randomPosition);
        
        
        foreach (var rig in rigidbodies)
        {
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection *= 0.1f;
            randomPosition += randomDirection;
            rig.transform.position = randomPosition;
            rig.velocity = Vector3.zero;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(_isGhost || _explode) return;
        
        // if it collides with the floor, make the ball explode and save all the rigidbodies found within its range, that will be later manipulated.
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Obstacles")) && (Vector3.Angle(other.contacts[0].normal, Vector3.up) < 15f))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            _explode = true;
            _bullet.gameObject.SetActive(false);
            SaveRigidbodiesInArea();
        }
    }

    /// <summary>
    /// Saves all rigidbodies within the given sphere to a list.
    /// </summary>
    private void SaveRigidbodiesInArea()
    {
        _suckedInRigidbodies.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, _forceRadius, _interactableLayerMask);

        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent(out Rigidbody rigidbody)) continue;
            
            _suckedInRigidbodies.Add(rigidbody);
        }
    }

    /// <summary>
    /// Makes the rigidbodies to move towards the center of the ball/black hole.
    /// </summary>
    private void SuckInRigidbodies(List<Rigidbody> rigidbodies)
    {
        foreach (var rig in rigidbodies)
        {
            rig.velocity *= 0.5f;
            var direction = transform.position - rig.transform.position;
            var distance = Vector3.Distance(transform.position ,rig.transform.position);
            rig.AddForce(direction * (_force * distance * 0.9f), ForceMode.Acceleration);
        }
    }
}
