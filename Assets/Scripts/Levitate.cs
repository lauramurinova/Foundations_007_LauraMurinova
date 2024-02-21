using UnityEngine;

public class Levitate : MonoBehaviour
{
    public float levitateHeight = 1f;    
    public float levitateSpeed = 1f;     

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * levitateSpeed) * levitateHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
