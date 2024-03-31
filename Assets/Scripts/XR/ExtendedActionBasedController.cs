using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ExtendedDirectInteractor))]
public class ExtendedActionBasedController : ActionBasedController
{
    [SerializeField] private ExtendedDirectInteractor _interactor;
    [SerializeField] private float _doublePressThreshold = 0.5f;
    
    private int _selectCount = 0;
    private DateTime _selectionTime;
    
    private void Start()
    {
        selectAction.action.performed += OnSelectPerformed;
    }

    private void OnSelectPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _selectCount++;

        if (_selectCount == 1)
        {
            UpdateSelectTime();
            return;
        }

        if (IsDoubleSelectTimeMet()){
            if (_selectCount == 2)
            {
                SendMessage("TriggerDoubleSelect", SendMessageOptions.RequireReceiver);
                _selectCount = 0;
            }
        }
        else
        {
            _selectCount = 1;
            UpdateSelectTime();
        }
    }

    private bool IsDoubleSelectTimeMet()
    {
        return (DateTime.Now - _selectionTime).TotalSeconds <= _doublePressThreshold;
    }

    private void UpdateSelectTime()
    {
        _selectionTime = DateTime.Now;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        activateAction.action.performed -= OnSelectPerformed;
    }
}
