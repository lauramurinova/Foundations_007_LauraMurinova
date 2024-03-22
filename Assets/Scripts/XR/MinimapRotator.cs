using UnityEngine;

public class MinimapRotator : MonoBehaviour
{

    [SerializeField] private Transform _rotationReference;

    private Vector3 _initialRotation;

    private void Start()
    {
        _initialRotation = transform.eulerAngles;
    }

    private void Update()
    {
        Vector3 newRot = new Vector3(0, 0, -_rotationReference.eulerAngles.y) + _initialRotation;
        transform.rotation = Quaternion.Euler(newRot);
    }
}
