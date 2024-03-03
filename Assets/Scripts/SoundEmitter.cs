using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundEmitter : MonoBehaviour
{
    [SerializeField] private float _soundRadius = 15f;
    [SerializeField] private float _impulseThreshold = 1f;
    
    private AudioSource _audioSource;
    
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.impulse.magnitude > _impulseThreshold || other.gameObject.CompareTag("Player"))
        {
            _audioSource.Play();
            
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

