using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MiniMapcCameraController : MonoBehaviour
{
   [SerializeField] private LayerMask[] _cullingLayerMasks;
   
   private Camera _camera;

   private int _currentView = 0;
   private void Start()
   {
      _camera = GetComponent<Camera>();
   }

   public void SwitchCullingLayerMask()
   {
      if(_cullingLayerMasks.Length == 0) return;

      if (_currentView + 1 >= _cullingLayerMasks.Length)
      {
         _currentView = 0;
      }
      else
      {
         _currentView++;
      }
      _camera.cullingMask = _cullingLayerMasks[_currentView];
   }
}
