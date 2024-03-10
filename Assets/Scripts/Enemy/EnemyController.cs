using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    enum EnemyState
    {
        Patrol = 0,
        Investigate = 1
    }

    public UnityEvent<Transform> onPlayerFound;
    public UnityEvent onInvestigate;
    public UnityEvent onReturnToPatrol;

    [SerializeField] private GameObject _robotGiblets;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private float _threshold = 0.5f;
    [SerializeField] private float _waitTime = 2f;
    [SerializeField] private PatrolRoute _patrolRoute;
    [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private EnemyState _state = EnemyState.Patrol;
    [SerializeField] private Material _frozenRobotMat;
    [SerializeField] private AudioSource _freezeAudio;

    private bool _moving = false;
    private Transform _currentPoint;
    private int _routeIndex = 0;
    private bool _forwardsAlongPath = true;
    private Vector3 _investigationPoint;
    private float _waitTimer = 0f;
    private bool _playerFound = false;
    private bool _destroyed = false;

    private void Start()
    {
        _currentPoint = _patrolRoute.route[_routeIndex];
    }

    void Update()
    {
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
        
        if (!_destroyed)
        {
            if (_fieldOfView.visibleObjects.Count > 0)
            {
                PlayerFound(_fieldOfView.visibleObjects[0].position);
            }

            if (_state.Equals(EnemyState.Patrol))
            {
                UpdatePatrol();
            }
            else if (_state.Equals(EnemyState.Investigate))
            {
                UpdateInvestigate();
            }
        }
    }

    /// <summary>
    /// Destroys the robot and makes it explode.
    /// </summary>
    public void Explode()
    {
        if (!_destroyed)
        {
            _destroyed = true;
            
            StartCoroutine(ExplodeTransition());
        }

    }

    /// <summary>
    /// Freezes and explodes the robot over time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExplodeTransition()
    {
        _agent.isStopped = true;
        _freezeAudio.Play();
        yield return new WaitForSeconds(0.5f);
        _renderer.material = _frozenRobotMat;
        yield return new WaitForSeconds(2f);
        var robot = Instantiate(_robotGiblets, transform.position, transform.rotation);
        ApplyExplosionForceToObject(robot);

        Destroy(gameObject);
    }

    /// <summary>
    /// Add explosion forces to each rigidBody of the object passed.
    /// </summary>
    private void ApplyExplosionForceToObject(GameObject robot)
    {
        foreach (var rigidbody in robot.GetComponentsInChildren<Rigidbody>())
        {
            rigidbody.AddExplosionForce(400f, rigidbody.transform.position, 2f);
        }
    }

    public void InvestigatePoint(Vector3 investigationPoint)
    {
        SetInvestigationPoint(investigationPoint);

        onInvestigate.Invoke();
    }

    private void SetInvestigationPoint(Vector3 investigationPoint)
    {
        _state = EnemyState.Investigate;
        _investigationPoint = investigationPoint;
        _agent.SetDestination(_investigationPoint);
    }

    private void PlayerFound(Vector3 investigatePoint)
    {
        if (_playerFound) return;
        
        SetInvestigationPoint(investigatePoint);
        
        onPlayerFound.Invoke(_fieldOfView.creature.head);

        _playerFound = true;
    }

    private void UpdateInvestigate()
    {
        if (Vector3.Distance(transform.position, _investigationPoint) <= _threshold)
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
        _waitTimer = 0f;
        _moving = false;
        
        onReturnToPatrol.Invoke();
    }

    private void UpdatePatrol()
    {
        if (!_moving)
        {
            NextPatrolPoint();
            _agent.SetDestination(_currentPoint.position);
            _moving = true;
        }

        if (_moving && Vector3.Distance(transform.position, _currentPoint.position) <= _threshold)
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
}
