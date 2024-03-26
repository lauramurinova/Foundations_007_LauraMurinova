using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FlashStepTeleportationProvider : TeleportationProvider
{
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _maxStepPossible = 0.25f;
    [SerializeField] private float _basicStepSize = 0.1f;
    [SerializeField] private float _destinationThreshold = 0.1f;
    [SerializeField] private GameObject _vignette;

    private XRRig _xrRig;
    private bool _isMoving = false;
    private Vector3 _targetPosition;
    private PlayerManager _player;

    private void Start()
    {
        _xrRig = system.xrRig;
        _player = system.xrRig.GetComponent<PlayerManager>();
        _player.playerDied.AddListener(delegate { Teleport(false); });
        VignetteEffect(false);
    }
    
    protected override void Update()
    {
        base.Update();

        if (_isMoving)
        {
            MoveToDestination();
        }
    }

    /// <summary>
    /// Gets the desired teleport destination for provider.
    /// </summary>
    public override bool QueueTeleportRequest(TeleportRequest teleportRequest)
    {
        _targetPosition = teleportRequest.destinationPosition;
        Teleport(true);
        return true;
    }

    /// <summary>
    /// Check whether the player can make the next step.
    /// </summary>
    private bool IsNextStepPossible(Vector3 direction, float step)
    {
        Vector3 newPosition = _xrRig.transform.position + direction * step;
        if (Math.Abs(newPosition.y - _xrRig.transform.position.y) > _maxStepPossible)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Called if player is moving.
    /// Enables continuous movement to the destination.
    /// </summary>
    private void MoveToDestination()
    {
        if (_xrRig != null)
        {
            Vector3 direction = (_targetPosition - _xrRig.transform.position).normalized;
            float step = _basicStepSize + _movementSpeed * Time.deltaTime;

            // if player cannot make the next step (too high) stop teleport
            if (!IsNextStepPossible(direction, step))
            {
                Teleport(false);
                return;
            }
            
            // make the step
            _xrRig.transform.position += direction * step;
            
            // if the player reached destination stop teleport
            if (Vector3.Distance(_xrRig.transform.position, _targetPosition) < _destinationThreshold)
            {
                Teleport(false);
            }
        }
    }

    /// <summary>
    /// Enables/Disables flash step movement based on the "enabled" parameter.
    /// </summary>
    private void Teleport(bool enabled)
    {
        _isMoving = enabled;
        VignetteEffect(enabled);
        if (!enabled)
        {
            _targetPosition = Vector3.zero;
        }
    }
    
    /// <summary>
    /// Enables/Disables vignette effect based on the "enabled" parameter.
    /// </summary>
    private void VignetteEffect(bool enabled)
    {
        _vignette.SetActive(enabled);
    }
}
