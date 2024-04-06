using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCustomTeleportationProvider : MonoBehaviour
{
    [SerializeField] private XRBaseInteractor _leftHandInteractor;
    [SerializeField] private XRBaseInteractor _rightHandInteractor;
    [SerializeField] private Animator _vignetteAnimator;
    public bool isTeleporting;

    private Transform _leftHandCurrentSelection;
    private Transform _rightHandCurrentSelection;

    private void Start()
    {
        _leftHandInteractor.selectEntered.AddListener(LeftHandEnter);
        _leftHandInteractor.selectExited.AddListener(LeftHandExit);
        _rightHandInteractor.selectEntered.AddListener(RightHandEnter);
        _rightHandInteractor.selectExited.AddListener(RightHandExit);
    }

    private void RightHandExit(SelectExitEventArgs arg0)
    {
        _rightHandCurrentSelection = UnbindGrab(_rightHandCurrentSelection);
    }

    private void RightHandEnter(SelectEnterEventArgs arg0)
    {
        _rightHandCurrentSelection = BindGrab(arg0.interactable);
    }

    private void LeftHandExit(SelectExitEventArgs arg0)
    {
        _leftHandCurrentSelection = UnbindGrab(_leftHandCurrentSelection);
    }

    private void LeftHandEnter(SelectEnterEventArgs arg0)
    {
        _leftHandCurrentSelection = BindGrab(arg0.interactable);
    }

    private Transform BindGrab(XRBaseInteractable interactable)
    {
        if (interactable is XRBaseInteractable)
        {
            return interactable.transform;
        }

        return null;
    }

    private Transform UnbindGrab(Transform currentSelection)
    {
        currentSelection.parent = null;
        return null;
    }

    private void ParentInteractable(XRBaseInteractor interactor, Transform currentSelection)
    {
        if (currentSelection) currentSelection.parent = interactor.transform;
    }
    
    private void UnparentInteractable(XRBaseInteractor interactor, Transform currentSelection)
    {
        if (currentSelection) currentSelection.parent = null;
    }

    public void TeleportBegin()
    {
        isTeleporting = true;
        _vignetteAnimator.SetBool("IsTeleporting", isTeleporting);
        ParentInteractable(_rightHandInteractor, _rightHandCurrentSelection);
        ParentInteractable(_leftHandInteractor, _leftHandCurrentSelection);
    }

    public void TeleportEnd()
    {
        isTeleporting = false;
        _vignetteAnimator.SetBool("IsTeleporting", isTeleporting);
        UnparentInteractable(_rightHandInteractor, _rightHandCurrentSelection);
        UnparentInteractable(_leftHandInteractor, _leftHandCurrentSelection);
    }
}
