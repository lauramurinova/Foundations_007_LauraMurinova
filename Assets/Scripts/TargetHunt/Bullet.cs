using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _maxLifeTime = 3f;
    [SerializeField] private GameObject _hitPsSystem;
    
    private float _lifeTimeTimer = 0f;
    
    /// <summary>
    /// Checks if the bullet collided with a target and makes it collapse.
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.TryGetComponent(out MovingTarget target))
        {
            float knockDownDistanceFactor = Vector3.Distance(GetCollisionCenter(other), target.GetCenter().position);
            target.KnockDown(knockDownDistanceFactor);
        };
        Instantiate(_hitPsSystem, GetCollisionCenter(other), Quaternion.identity);
        DestroyBullet();
    }

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

    /// <summary>
    /// Destroys the bullet gameObject.
    /// </summary>
    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

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
}
