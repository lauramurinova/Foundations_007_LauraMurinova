using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float threshold = 0.5f;

    [SerializeField] private Transform destinationPointsParent;
    private List<Transform> points = new List<Transform>();
    
    private bool _moving = false;
    private Transform _currentPoint;

    /// <summary>
    /// Ensures dynamic loading of points.
    /// </summary>
    private void Start()
    {
        LoadDestinationPoints();
    }

    /// <summary>
    /// Ensures the enemy to pend between multiple point positions.
    /// </summary>
    void Update()
    {
        if (!_moving)
        {
            agent.SetDestination(GetNextDestinationPoint().position);
            _moving = true;
        }

        if (_moving && Vector3.Distance(transform.position, _currentPoint.position) < threshold)
        {
            _moving = false;
        }
    }

    /// <summary>
    /// Loads dynamically destination points from a parent and loads them into the points list.
    /// </summary>
    private void LoadDestinationPoints()
    {
        foreach (Transform destinationPoint in destinationPointsParent)
        {
            points.Add(destinationPoint);
        }
    }

    /// <summary>
    /// Returns a random destination point from the points list.
    /// </summary>
    private Transform GetNextDestinationPoint()
    {
        int randomPointPosition = Random.Range(0, points.Count);
        while (points.Count > 1 && points[randomPointPosition] == _currentPoint)
        {
            randomPointPosition = Random.Range(0, points.Count);
        }

        _currentPoint = points[randomPointPosition];
        return _currentPoint;
    }
}
