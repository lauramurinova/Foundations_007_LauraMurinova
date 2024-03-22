using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]

public class SoundEmitter : MonoBehaviour
{
    public UnityEvent onEmitSound;
    
    [SerializeField] private float _soundRadius = 5f;
    [SerializeField] private float _impulseThreshold = 2f;

    private float _collisionTimer = 0f;
    
    private AudioSource _audioSource;
    
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_collisionTimer < 2f)
        {
            _collisionTimer += Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_collisionTimer < 2f) return;
        
        if (other.impulse.magnitude > _impulseThreshold || other.gameObject.CompareTag("Player"))
        {
            Debug.Log(other.gameObject.name);
            _audioSource.Play();
            onEmitSound.Invoke();
            
            Collider[] _colliders = Physics.OverlapSphere(transform.position, _soundRadius);
            foreach (var collider in _colliders)
            {
                if (collider.TryGetComponent(out EnemyController enemyController))
                {
                    enemyController.InvestigatePoint(transform.position);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _soundRadius);
    }
}
