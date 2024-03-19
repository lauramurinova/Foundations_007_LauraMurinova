using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
    /// Called when bullet collided with the target.
    /// Invokes listener with calculated score, based on the distance from the center point.
    /// Sets proper rotation to object.
    /// </summary>
    public void KnockDown(float hitDistanceFactor)
    {
        if(_knockedDown) return;
        
        transform.localEulerAngles += new Vector3(80, 00, 0);
        _knockedDown = true;
        int score = (int)(_baseScore * 1/hitDistanceFactor);
        knockedDownEvent.Invoke(score);
    }

    /// <summary>
    /// Resets the object to initial state.
    /// </summary>
    public void ResetToInitial()
    {
        _currentWayPointIndex = 0;
        transform.localEulerAngles -= new Vector3(80, 0, 0);
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
}
