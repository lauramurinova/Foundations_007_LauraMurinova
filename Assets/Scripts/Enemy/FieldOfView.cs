using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public List<Transform> visibleObjects;
    
    [SerializeField] private Color _gizmoColor = Color.red;
    [SerializeField] private float _viewRadius = 6f;
    [SerializeField] private float _viewAngle = 30f;
    [SerializeField] private Creature _creature;
    [SerializeField] private LayerMask _blockingLayers;

    private void Update()
    {
        visibleObjects.Clear();
        
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius);
        foreach (Collider target in targetsInViewRadius)
        {
            if (!target.TryGetComponent(out Creature targetCreature)) continue;
            
            if(_creature.team.Equals(targetCreature.team)) continue;
            
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < _viewAngle)
            {
                Vector3 headPos = _creature.head.position;
                Vector3 targetHeadPos = targetCreature.head.position;

                Vector3 dirToTargetHead = (targetHeadPos - headPos).normalized;
                float distToTargetHead = Vector3.Distance(headPos, targetHeadPos);

                if (Physics.Raycast(headPos, dirToTargetHead, distToTargetHead, _blockingLayers))
                {
                    continue;
                }
                
                Debug.DrawLine(headPos, targetHeadPos, Color.cyan);
                
                visibleObjects.Add(target.transform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = _gizmoColor;

        Handles.DrawWireArc(transform.position, transform.up, transform.forward, _viewAngle, _viewRadius);
        Handles.DrawWireArc(transform.position, transform.up, transform.forward, -_viewAngle, _viewRadius);
        
        Vector3 lineA = Quaternion.AngleAxis(_viewAngle, transform.up) * transform.forward;
        Vector3 lineB = Quaternion.AngleAxis(-_viewAngle, transform.up) * transform.forward;
        Handles.DrawLine(transform.position, transform.position + (lineA * _viewRadius));
        Handles.DrawLine(transform.position, transform.position + (lineB * _viewRadius));
    }
}
