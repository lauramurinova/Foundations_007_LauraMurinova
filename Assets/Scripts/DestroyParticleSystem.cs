using UnityEngine;

public class DestroyParticleSystem : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private AudioSource _audioSource;

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!_particleSystem.IsAlive() && !_audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
