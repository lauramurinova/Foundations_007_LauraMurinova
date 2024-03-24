using UnityEngine;

public class SwingingBlade : MonoBehaviour
{
    [SerializeField] private float _swingSpeed = 2f; 
    [SerializeField] private float _swingAngle = 15f;
    [SerializeField] private AudioSource _audioSource;

    private Vector3 _originalEuler;

    private void Start()
    {
        _originalEuler = transform.localEulerAngles;
    }

    private void Update()
    {
        // animate the blade swinging in specific given angle
        float swingAngle = Mathf.Sin(Time.time * _swingSpeed) * _swingAngle;
        transform.rotation = Quaternion.Euler( _originalEuler.x + swingAngle , _originalEuler.y, _originalEuler.z);
        if (swingAngle > 1f && !_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }
}
