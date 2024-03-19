using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _maxLifeTime = 3f;
    [SerializeField] private ParticleSystem _hitPsSystem;
    
    private float _lifeTimeTimer = 0f;
    
    /// <summary>
    /// Ensures the bullet's position and if it hits nothing, destroys it within the set max life time.
    /// </summary>
    private void FixedUpdate()
    {
        transform.Translate(-Vector3.forward * (_speed * Time.deltaTime));

        if (_lifeTimeTimer > _maxLifeTime)
        {
            DestroyBullet();
        }
        else
        {
            _lifeTimeTimer += Time.deltaTime;
        }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        _hitPsSystem.transform.position = GetCollisionCenter(other);
        _hitPsSystem.Play();
    }
    
    /// <summary>
    /// Returns the center point of collision.
    /// </summary>
    private Vector3 GetCollisionCenter(Collision collision)
    {
        Vector3 centerPoint = Vector3.zero;
        foreach (ContactPoint contact in collision.contacts)
        {
            centerPoint += contact.point;
        }
        centerPoint /= collision.contacts.Length;
        return centerPoint;
    }

    /// <summary>
    /// Destroys the bullet gameObject.
    /// </summary>
    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
