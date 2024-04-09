using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR.Interaction.Toolkit;

public class XRRotationInteractable : XRBaseInteractable
{

    [SerializeField] private Transform _rotationReference;

    private void Start()
    {
        Assert.IsNotNull(_rotationReference, "You have not assigned rotation reference to " + name);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        _rotationReference.transform.SetParent(selectingInteractor.transform);
        _rotationReference.transform.position = selectingInteractor.transform.position;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _rotationReference.transform.SetParent(transform);
        _rotationReference.transform.rotation = transform.rotation;
        _rotationReference.transform.position = transform.position;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if(!isSelected) return;
        
        Rotate();
    }

    private void Rotate()
    {
        Vector3 projectedVector = Vector3.ProjectOnPlane(_rotationReference.up, -transform.forward);
        transform.rotation = Quaternion.LookRotation(transform.forward, projectedVector);
    }
}