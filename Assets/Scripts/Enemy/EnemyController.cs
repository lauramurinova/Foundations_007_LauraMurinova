using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    enum EnemyState
    {
        Patrol = 0,
        Investigate = 1,
        FindAlly = 2
    }
    
    public bool isAlly = false;
    
    [SerializeField] private Color _gizmoColor = Color.red;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _transform;
    [SerializeField] private float _threshold = 0.5f;
    [SerializeField] private float _hearingRadius = 10f;
    [SerializeField] private float _waitTime = 2f;
    [SerializeField] private PatrolRoute _patrolRoute;
    [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private EnemyState _state = EnemyState.Patrol;
    [SerializeField] private GameObject _questionMarks;

    private bool _moving = false;
    private Transform _currentPoint;
    private int _routeIndex = 0;
    private bool _forwardsAlongPath = true;
    private Vector3 _investigationPoint;
    private float _waitTimer = 0f;
    private EnemyController _closestAlly;
    private Coroutine _transitionCoroutine;

    private void Start()
    {
        _currentPoint = _patrolRoute.route[_routeIndex];
    }

    void Update()
    {
        if (_fieldOfView.visibleObjects.Count > 0)
        {
            InvestigatePoint(_fieldOfView.visibleObjects[0].position);
        }
        
        UpdateBasedOnState();
    }

    private void UpdateBasedOnState()
    {
        switch (_state)
        {
            case EnemyState.Patrol:
            {
                UpdatePatrol();
                DetectSound();
                break;
            }
            case EnemyState.Investigate:
            {
                UpdateInvestigate();
                break;
            }
            case EnemyState.FindAlly:
            {
                UpdateFindAlly();
                break;
            }
        }
    }
    
    /// <summary>
    /// Detects all sounds within the hearing radius of the enemy, if it detects sound, it tries to find an ally to go to investigate with.
    /// If ally is found, start investigation of the point with the ally.
    /// </summary>
    private void DetectSound()
    {
        Collider[] _colliders = Physics.OverlapSphere(_transform.position, _hearingRadius);
        foreach (var collider in _colliders)
        {
            if (collider.TryGetComponent(out AudioSource audioSource))
            {
                if (audioSource.isPlaying)
                {
                    if (Random.value > 0.5f && EnemyManager.GetInstance().CanInvestigatePoint(collider.transform.position))
                    {
                        if (TryFindClosestAvailableAlly())
                        {
                            _investigationPoint = collider.transform.position;
                            StartInvestigationWithAlly();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Starts the point investigation with the closest found ally.
    /// Marks the point as investigated and the triggers enemy to go to the ally.
    /// </summary>
    private void StartInvestigationWithAlly()
    {
        EnemyManager.GetInstance().MarkInvestigationPoint(_investigationPoint);
        _questionMarks.SetActive(true);
        _state = EnemyState.FindAlly;
        _closestAlly.isAlly = true;
        float distanceToGo = Vector3.Distance(_transform.position, _investigationPoint);
        EnemyManager.GetInstance().StartCoolDown(_investigationPoint, distanceToGo);
        _transitionCoroutine ??= StartCoroutine(TransitionState(_investigationPoint, 7f));
    }

    /// <summary>
    /// Finds the nearest available ally. Returns true if it was found, false otherwise.
    /// </summary>
    private bool TryFindClosestAvailableAlly()
    {
        float closestDistance = float.MaxValue;
        foreach (EnemyController ally in EnemyManager.GetInstance().GetEnemies())
        {
            float distance = Vector3.Distance(_transform.position, ally._transform.position);

            if (distance < closestDistance && !ally.Equals(this) && ally._state.Equals(EnemyState.Patrol) && !ally.isAlly)
            {
                closestDistance = distance;
                _closestAlly = ally;
            }
        }
        return (_closestAlly != null);
    }

    /// <summary>
    /// Enables smooth transition between states.
    /// </summary>
    private IEnumerator TransitionState(Vector3 lookAtTransform, float speed)
    {
        _agent.isStopped = true;
        yield return new WaitForSeconds(0.25f);
        yield return RotateSmoothly(lookAtTransform, 0.5f);
        yield return new WaitForSeconds(1.5f);
        _agent.speed = speed;
        _agent.isStopped = false;
        _transitionCoroutine = null;
    }
    
    /// <summary>
    /// Enables smooth rotation towards a certain transform during set time.
    /// </summary>
    private IEnumerator RotateSmoothly(Vector3 lookAtTransform, float duration)
    {
        Quaternion startRotation = _transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(lookAtTransform - _transform.position);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
            yield return null;
        }

        _transform.rotation = targetRotation;
    }
    
    /// <summary>
    /// Enables the transition/movement to the closest ally. Called by update.
    /// </summary>
    private void UpdateFindAlly()
    {
        _agent.SetDestination(_closestAlly._transform.position);
        if (Vector3.Distance(_transform.position, _closestAlly._transform.position) <= _threshold)
        {
            GoInvestigateWithAlly();
        }
    }
    
    /// <summary>
    /// Triggers enemy and his ally to go investigate the point (the sound).
    /// </summary>
    private void GoInvestigateWithAlly()
    {
        InvestigatePoint(_investigationPoint);
        _closestAlly.InvestigatePoint(_investigationPoint);
        
        _transitionCoroutine ??= StartCoroutine(TransitionEnemyAndAlly());
    }

    /// <summary>
    /// Enables smooth transition to the investigation with the ally - they look at each other and then continue investigate.
    /// </summary>
    private IEnumerator TransitionEnemyAndAlly()
    {
        StartCoroutine(TransitionState(_closestAlly.transform.position, 3.5f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(_closestAlly.TransitionState(transform.position, 3.5f));
        _transitionCoroutine = null;
    }

    public void InvestigatePoint(Vector3 investigationPoint)
    {
        _state = EnemyState.Investigate;
        _investigationPoint = investigationPoint;
        _agent.SetDestination(_investigationPoint);
    }

    private void UpdateInvestigate()
    {
        if (Vector3.Distance(_transform.position, _investigationPoint) <= _threshold)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer > _waitTime)
            {
                ReturnToPatrol();
            }
        }
    }

    private void ReturnToPatrol()
    {
        _state = EnemyState.Patrol;
        _closestAlly = null;
        isAlly = false;
        _waitTimer = 0f;
        _moving = false;
        if (_questionMarks.activeSelf)
        {
            _questionMarks.SetActive(false);
        }
    }

    private void UpdatePatrol()
    {
        if (!_moving)
        {
            NextPatrolPoint();
            _agent.SetDestination(_currentPoint.position);
            _moving = true;
        }

        if (_moving && Vector3.Distance(_transform.position, _currentPoint.position) <= _threshold)
        {
            _moving = false;
        }
    }

    private void NextPatrolPoint()
    {
        if (_forwardsAlongPath)
        {
            _routeIndex++;
        }
        else
        {
            _routeIndex--;
        }

        if (_routeIndex.Equals(_patrolRoute.route.Count))
        {
            if (_patrolRoute.patrolType.Equals(PatrolRoute.PatrolType.Loop))
            {
                _routeIndex = 0;
            }
            else
            {
                _forwardsAlongPath = false;
                _routeIndex-=2;
            }
        }

        if (_routeIndex == 0)
        {
            _forwardsAlongPath = true;
        }
            
        _currentPoint = _patrolRoute.route[_routeIndex];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireSphere(_transform.position, _hearingRadius);
    }
}
