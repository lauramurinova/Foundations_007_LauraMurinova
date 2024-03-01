using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ProximityMine : MonoBehaviour
{
    [SerializeField] private float maxDistanceFromWall = 3.5f;
    [SerializeField] private float explosionSize = 7f;
    [SerializeField] private LayerMask robotMask;
    [SerializeField] private GameObject explosionParticleObject;
    [SerializeField] private AudioSource explosionAudio;

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
        if (Physics.Raycast(transform.position, transform.forward, out var hit, maxDistanceFromWall, robotMask))
        {
            if (hit.transform.gameObject.TryGetComponent(out Creature creature))
            {
                if (creature.team.Equals(Creature.Team.Enemy))
                {
                    DisableEnemy(creature.gameObject);
                    DisableMine();
                }
            }
        }
    }

    /// <summary>
    /// Called when mine was thrown, checks if it can stick to a wall, if so the mine attaches to the wall
    /// at the point where raycaster from it hits the wall.
    /// </summary>
    private void TryToStick()
    {
        if (Physics.Raycast(transform.position, -transform.forward, out var hit, maxDistanceFromWall))
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
    /// Disables the enemy robot and plays particle effects.
    /// </summary>
    [Button]
    private void DisableEnemy(GameObject enemy)
    {
        Instantiate(explosionParticleObject, _transform.position, _transform.rotation, _transform).transform.localScale *= explosionSize;
        explosionAudio.Play();
        enemy.SetActive(false);
    }

    /// <summary>
    /// Disables the mine (after explosion).
    /// </summary>
    private void DisableMine()
    {
        _isStick = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + -transform.forward * maxDistanceFromWall);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * maxDistanceFromWall);
    }
}
