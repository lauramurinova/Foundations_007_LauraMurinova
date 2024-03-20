using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MovingTarget : MonoBehaviour
{
    public UnityEvent<int> knockedDownEvent = new UnityEvent<int>();
    
    [SerializeField] private Transform[] _wayPoints;
    [SerializeField] private Transform _center;
    [SerializeField] private float _speed = 0.1f;
    [SerializeField] private int _baseScore = 10;
    
    private bool _knockedDown = false;
    private int _currentWayPointIndex = 0;

    /// <summary>
    /// Ensures the transition between set WayPoints, if the target hasn't been knocked down.
    /// </summary>
    private void FixedUpdate()
    {
        if (_wayPoints.Length == 0 || _knockedDown) return;

        if (Vector3.Distance(transform.position, _wayPoints[_currentWayPointIndex].position) < 0.01f)
        {
            _currentWayPointIndex++;
            if (_currentWayPointIndex >= _wayPoints.Length)
            {
                _currentWayPointIndex = 0;
            }
        }
        
        transform.position = Vector3.MoveTowards(transform.position, _wayPoints[_currentWayPointIndex].position, _speed * Time.deltaTime);
    }
    
    /// <summary>
    /// Checks if the bullet collided with a target and makes it collapse.
    /// </summary>
    private void OnCollisionEnter(Collision other)
    {
        if(!other.transform.GetComponent<Bullet>()) return;
        
        float knockDownDistanceFactor = Vector3.Distance(GetCollisionCenter(other), GetCenter().position);
        KnockDown(knockDownDistanceFactor);
    }

    /// <summary>
    /// Called when bullet collided with the target.
    /// Invokes listener with calculated score, based on the distance from the center point.
    /// Sets proper rotation to object.
    /// </summary>
    public void KnockDown(float hitDistanceFactor)
    {
        if(_knockedDown) return;
        
        StartCoroutine(RotateObjectByEuler(new Vector3(0f, 80f, 0f), 0.25f));
        _knockedDown = true;
        int score = (int)(_baseScore * 1/hitDistanceFactor);
        knockedDownEvent.Invoke(score);
    }
    
    /// <summary>
    /// Rotate object continuously by the desired rotation angle.
    /// </summary>
    private IEnumerator RotateObjectByEuler(Vector3 targetRotation, float duration)
    {
        Quaternion startRotation = transform.localRotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(targetRotation);
        
        float counter = 0f;

        while (counter < duration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, counter / duration);
            counter += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = endRotation;
    }

    /// <summary>
    /// Resets the object to initial state.
    /// </summary>
    public void ResetToInitial()
    {
        _currentWayPointIndex = 0;
        StartCoroutine(RotateObjectByEuler(new Vector3(0f, -80f, 0f), 0.5f));
        _knockedDown = false;
    }

    /// <summary>
    /// Returns true if the can has been knocked down, otherwise false.
    /// </summary>
    public bool IsKnockedDown()
    {
        return _knockedDown;
    }

    //Returns center point of the target (where the player should aim).
    public Transform GetCenter()
    {
        return _center;
    }
    
    /// <summary>
    /// Returns the center point of collision.
    /// </summary>
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
