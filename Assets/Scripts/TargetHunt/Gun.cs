using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private float _fireCountDown = 0.25f;
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _shootPoint;

    private float _fireTimer = 0f;

    /// <summary>
    /// Call on Grab Activate.
    /// Fires bullet from a gun if cool time passed.
    /// </summary>
    public void FireBullet()
    {
        if (_fireTimer < _fireCountDown) return;
      
        if (_fireTimer < _fireCountDown) return;
      
        Instantiate(_bullet, _shootPoint.position, transform.rotation);
        _audioSource.Play();
        _fireTimer = 0f;
        _rigidbody.AddForce(transform.forward * 3f, ForceMode.Impulse);
    }

    private void Update()
    {
        _fireTimer += Time.deltaTime;
    }
}
