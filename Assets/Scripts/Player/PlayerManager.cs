using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    private enum PlayerState
    {
        Alive, Dying, Dead
    }

    public UnityEvent playerDied;

    [SerializeField] private Flammable _flammable;
    [SerializeField] private GameObject _hit;
    [SerializeField] private float _lifeTime = 10f;
    
    private PlayerState _state = PlayerState.Alive;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private float _lifeTimer;

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

        _flammable.igniteEvent.AddListener(PlayerDying);
        _flammable.extinguishEvent.AddListener(PlayerHeals);
    }

    private void Update()
    {
        switch (_state)
        {
            case PlayerState.Dying:
            {
                UpdatePlayerDying();
                break;
            }
        }
    }

    private void UpdatePlayerDying()
    {
        _lifeTimer += Time.deltaTime;
        CheckIfPlayerDied();
    }

    private void CheckIfPlayerDied()
    {
        if (_lifeTimer > _lifeTime && _state.Equals(PlayerState.Dying))
        {
            PlayerDies();
        }
    }

    private void PlayerDying()
    {
        _state = PlayerState.Dying;
        _hit.SetActive(true);
    }

    private void PlayerDies()
    {
        _hit.SetActive(false);
        _state = PlayerState.Dead;
        playerDied.Invoke();
    }

    private void PlayerHeals()
    {
        _lifeTimer = 0;
        _hit.SetActive(false);
        _state = PlayerState.Alive;
    }

    private void OnCollisionEnter(Collision other)
    {
        // player dies when hit by a blade
        if(!other.gameObject.GetComponent<SwingingBlade>() || _state.Equals(PlayerState.Dead)) return;

        _state = PlayerState.Dead;
        playerDied.Invoke();
    }
}
