using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float threshold = 0.5f;
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;

    private bool _moving = false;
    private Transform _currentPoint;

    void Update()
    {
        if (!_moving)
        {
            if (_currentPoint == point1)
            {
                _currentPoint = point2;
            }
            else
            {
                _currentPoint = point1;
            }
            
            agent.SetDestination(_currentPoint.position);
            _moving = true;
        }

        if (_moving && Vector3.Distance(transform.position, _currentPoint.position) < threshold)
        {
            _moving = false;
        }
    }
}
