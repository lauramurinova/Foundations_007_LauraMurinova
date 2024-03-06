using Sirenix.OdinInspector;
using UnityEngine;

public class ProximityMine : MonoBehaviour
{
    [SerializeField] private float mineMaxDistanceDetection = 3.5f;
    [SerializeField] private float explosionSize = 3f;
    [SerializeField] private LayerMask robotMask;
    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody _rigidBody;
    private Transform _transform;
    private bool _isStick = false;
    private Grabbable _grabbable;
    
    private void Start()
    {
        _grabbable = GetComponent<Grabbable>();
        _rigidBody = GetComponent<Rigidbody>();
        _transform = transform;
        
        //Set listeners for grab and throw
        _grabbable.throwEvent.AddListener(TryToStick);
        _grabbable.grabEvent.AddListener(UnStick);
    }

    private void Update()
    {
        // if the mine is on the wall, check if robot passes in front of it.
        if (_isStick)
        {
            CheckForEnemyRobot();
        }
    }

    /// <summary>
    /// Raycaster checks if Enemy - Robot passes in front of the mine, if so, it disables him with effects.
    /// </summary>
    private void CheckForEnemyRobot()
    {
        if (Physics.Raycast(transform.position, transform.forward, out var hit, mineMaxDistanceDetection, robotMask))
        {
            if (hit.transform.gameObject.TryGetComponent(out EnemyController enemy))
            {
                DestroyEnemy(enemy);
                DestroyMine();
            }
        }
    }

    /// <summary>
    /// Called when mine was thrown, checks if it can stick to a wall, if so the mine attaches to the wall
    /// at the point where raycaster from it hits the wall.
    /// </summary>
    private void TryToStick()
    {
        if (Physics.Raycast(transform.position, -transform.forward, out var hit, mineMaxDistanceDetection))
        {
            if (hit.transform.gameObject.layer.Equals(LayerMask.NameToLayer("Obstacles")))
            {
                _rigidBody.isKinematic = true;
                _transform.position = hit.point;
                _transform.LookAt(_transform.position + hit.normal, Vector3.up);
                _transform.SetParent(hit.transform);
                _isStick = true;
            }
        }
    }

    /// <summary>
    /// Called when mine was grabbed, unsticks it from the wall, if it was stick.
    /// </summary>
    private void UnStick()
    {
        if (_isStick)
        {
            _rigidBody.isKinematic = false;
            _isStick = false;
        }
    }

    
    /// <summary>
    /// Destroys the enemy (to ragdoll state).
    /// </summary>
    private void DestroyEnemy(EnemyController enemy)
    {
        EnemyManager.GetInstance().DestroyRobot(enemy);
    }

    /// <summary>
    /// Destroys the mine with particle and sound effect.
    /// </summary>
    [Button]
    private void DestroyMine()
    {
        Instantiate(explosionPrefab, _transform.position, _transform.rotation).transform.localScale *= explosionSize;
        Destroy(gameObject);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + -transform.forward * mineMaxDistanceDetection);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * mineMaxDistanceDetection);
    }
}
