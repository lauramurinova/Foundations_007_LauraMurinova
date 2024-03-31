using Sirenix.OdinInspector;
using UnityEngine;

public class GrenadeBall : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    
    [Button]
    public void Init(Vector3 velocity)
    {
        _rigidbody.AddForce(velocity, ForceMode.Impulse);
    }
}
