using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SmoothTeleportationAnchor : BaseTeleportationInteractable
{
    [SerializeField] private float _teleportSpeed = 3f;
    [SerializeField] private float _stoppingDistance = 0.1f;

    private Vector3 _teleportEnd;
    private bool _isTeleporting;
    private XRRig _rig;
    private XRCustomTeleportationProvider _teleportationProvider;

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        BeginTeleport(args.interactor);
    }

    private void BeginTeleport(XRBaseInteractor interactor)
    {
        _rig = interactor.GetComponentInParent<XRRig>();
        _teleportationProvider = _rig.GetComponent<XRCustomTeleportationProvider>();
        
        if(_teleportationProvider.isTeleporting) return;
        _teleportationProvider.TeleportBegin();
        
        var interactorPos = interactor.transform.localPosition;
        interactorPos.y = 0;
        _teleportEnd = transform.position - interactorPos;
        _isTeleporting = true;
    }

    private void Update()
    {
        if (_isTeleporting)
        {

            _rig.transform.position = Vector3.MoveTowards(_rig.transform.position, _teleportEnd,
                _teleportSpeed * Time.deltaTime);

            if (Vector3.Distance(_rig.transform.position, _teleportEnd) < _stoppingDistance)
            {
                _isTeleporting = false;
                _teleportationProvider.TeleportEnd();
            }
        }
    }
}
