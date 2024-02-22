using UnityEngine;

public class Crouch : MonoBehaviour
{

    [SerializeField]
    private CharacterController _characterController;

    [SerializeField] private float _crouchHeight = 1f;

    private float _originalHeight;
    private bool _crouched = false;
    
    void Start()
    {
        _originalHeight = _characterController.height;
    }

    void Update()
    {
        
    }

    private void OnCrouch()
    {
        if (_crouched)
        {
            _crouched = false;
            _characterController.height = _originalHeight;
        }
        else
        {
            _crouched = true;
            _characterController.height = _crouchHeight;
        }
    }
}
