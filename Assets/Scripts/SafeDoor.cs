using System;
using UnityEngine;
using UnityEngine.Assertions;

public class SafeDoor : MonoBehaviour
{
    [SerializeField] private HingeJoint _doorJoint;
    [SerializeField] private AudioSource _doorOpenAudio;

    private Vector3 _lastDoorRotationEuler;
    
    private void Start()
    {
        Assert.IsNotNull(_doorJoint, "You have not assigned a door joint to the safe lock of object " + name);
    }
    
    /// <summary>
    /// Unlocked the door - sets the joint in a way, the player is able to push it.
    /// </summary>
    public void Open()
    {
        JointLimits jointLimits = _doorJoint.limits;
        jointLimits.max = 90f;
        _doorJoint.limits = jointLimits;
        _lastDoorRotationEuler = transform.eulerAngles;
    }

    private void OnCollisionEnter(Collision other)
    {
        // its just for an effect, plays a sound when the user tries to open the door
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Hand")) && Math.Abs(_lastDoorRotationEuler.y - transform.eulerAngles.y) > 5f)
        {
            _lastDoorRotationEuler = transform.eulerAngles;
            _doorOpenAudio.Play();
        }
    }
}
