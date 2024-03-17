using UnityEngine;

public class TossBall : MonoBehaviour
{
    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = transform.position;
    }

    /// <summary>
    /// Resets the transform position to initial.
    /// </summary>
    public void ResetToInitial()
    {
        transform.position = _initialPosition;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
