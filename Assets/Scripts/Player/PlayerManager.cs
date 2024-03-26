using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    private enum PlayerState
    {
        Alive, Dead
    }

    public UnityEvent playerDied;
    
    private PlayerState _state = PlayerState.Alive;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    /// <summary>
    /// Resets transform to initial state.
    /// </summary>
    public void ResetToInitial()
    {
        transform.position = _initialPosition;
        transform.rotation = _initialRotation;
    }

    private void Start()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        // player dies when hit by a blade
        if(!other.gameObject.GetComponent<SwingingBlade>() || _state.Equals(PlayerState.Dead)) return;

        _state = PlayerState.Dead;
        playerDied.Invoke();
    }
}
