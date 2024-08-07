using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class PatrolRoute : MonoBehaviour
{
    public enum PatrolType
    {
        Loop = 0,
        PingPong = 1
    }

    [FormerlySerializedAs("patrolRouteColor")] [SerializeField] private Color _patrolRouteColor = Color.green;
    
    public PatrolType patrolType;
    public List<Transform> route;

    [Button("Add Patrol Point")]
    private void AddPatrolPoint()
    {
        GameObject thisPoint = new GameObject();
        thisPoint.transform.position = transform.position;
        thisPoint.transform.parent = transform;
        thisPoint.name = "Point" + (route.Count + 1);
        route.Add(thisPoint.transform);
        
        #if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(thisPoint, "Add Patrol Point");
        #endif
    }

    [Button("Reverse Patrol Route")]
    private void ReversePatrolRoute()
    {
        route.Reverse();
    }

    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
            Handles.Label(transform.position, gameObject.name);
        #endif
        
        Gizmos.color = _patrolRouteColor;
        
        for (int i = 0; i < route.Count - 1; i++)
        {
            Gizmos.DrawLine(route[i].position, route[i+1].position);
        }

        if (patrolType.Equals(PatrolType.Loop))
        {
            Gizmos.DrawLine(route[route.Count-1].position, route[0].position);
        }
    }
}
