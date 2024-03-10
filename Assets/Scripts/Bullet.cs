using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _maxLifeTime = 1f;
    [SerializeField] private ParticleSystem _particleSystem;
    
    private float _lifeTimeTimer = 0f;
    
    /// <summary>
    /// Checks if the bullet collided with an enemy, if so, destroy him.
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.TryGetComponent(out EnemyController enemyController))
        {
            enemyController.Explode();
        };
        DestroyBullet();
    }

    /// <summary>
    /// Ensures the bullet's position and if it hits nothing, destroys it within the set max life time.
    /// </summary>
    private void Update()
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

    /// <summary>
    /// Destroys the bullet gameObject.
    /// </summary>
    private void DestroyBullet()
    {
        _particleSystem.Stop();
        Destroy(gameObject);
    }
}
