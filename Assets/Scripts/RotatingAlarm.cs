using UnityEngine;

public class RotatingAlarm : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
