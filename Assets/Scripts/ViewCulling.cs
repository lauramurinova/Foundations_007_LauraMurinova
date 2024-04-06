using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class ViewCulling : MonoBehaviour
{
    private FieldOfView _fov;
    private List<Transform> _enemiesInViewLastFrame;
    
    void Start()
    {
        _fov = GetComponent<FieldOfView>();
        _enemiesInViewLastFrame = new List<Transform>();
    }

    void Update()
    {
        // Out of view
        var enemiesOutOfView = _enemiesInViewLastFrame.Except(_fov.visibleObjects).ToList();
        foreach (var enemy in enemiesOutOfView)
        {
            var rend = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
            if (rend)
            {
                rend.enabled = false;
            }
        }
        
        // In the view
        var enemiesInView = _fov.visibleObjects.Except(_enemiesInViewLastFrame).ToList();
        foreach (var enemy in enemiesInView)
        {
            var rend = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
            if (rend)
            {
                rend.enabled = true;
            }
        }

        _enemiesInViewLastFrame = new List<Transform>(_fov.visibleObjects);
    }
}
