using UnityEngine;

public class TossBall : MonoBehaviour
{
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
    }

    public void ResetToInitial()
    {
        transform.position = _initialPosition;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
