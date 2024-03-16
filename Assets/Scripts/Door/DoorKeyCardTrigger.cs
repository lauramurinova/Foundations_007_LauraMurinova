using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorKeyCardTrigger : DoorTrigger
{
    [SerializeField] private int _keycardLevel = 1;

    [SerializeField] private XRSocketInteractor _socket;
    [SerializeField] private Renderer _lightObject;
    [SerializeField] private Light _light;
    [SerializeField] private Color _defaultColor = Color.yellow;
    [SerializeField] private Color _errorColor = Color.red;
    [SerializeField] private Color _successColor = Color.green;
    
    private bool _isOpen = false;

    private void Start()
    {
        SetLightColor(_defaultColor);
        
        _socket.selectEntered.AddListener(KeycardInserted);
        _socket.selectExited.AddListener(KeycardRemoved);
    }

    private void KeycardRemoved(SelectExitEventArgs arg0)
    {
        SetLightColor(_defaultColor);
        _isOpen = false;
        CloseDoor();
    }

    private void KeycardInserted(SelectEnterEventArgs arg0)
    {
        if(!arg0.interactable.TryGetComponent(out KeyCard keycard))
        {
            Debug.LogWarning("No Keycard attached to inserted object");
            SetLightColor(_defaultColor);
            return;
        }

        if (keycard.keycardLevel >= _keycardLevel)
        {
            SetLightColor(_successColor);
            _isOpen = true;
            OpenDoor();
        }
        else
        {
            SetLightColor(_errorColor);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (_isOpen) return;
        
        base.OnTriggerExit(other);
    }

    private void SetLightColor(Color color)
    {
        _lightObject.material.color = color;
        _light.color = color;
    }
}
