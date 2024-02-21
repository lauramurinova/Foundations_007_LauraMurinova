using UnityEngine;
using Interactables;

/// <summary>
/// Handles player interactions with objects in a Unity environment.
/// </summary>
public class GrabController : MonoBehaviour
{
    [SerializeField]
    private InteractableInputs interactions;
    private GrabbableObject hitGrabbableObject = null;
    private GrabbableObject grabbedObject = null;

    [SerializeField]
    private Transform hand;

    /// <summary>
    /// Updates each frame, calls check for grabbable objects in the area and user input for grab/drop.
    /// </summary>
    private void Update()
    {
        CheckForHitGrabbableObject();

        if (hitGrabbableObject && !grabbedObject && interactions.grab)
        {
            grabbedObject = hitGrabbableObject;
            grabbedObject.PickUp(hand);
        }

        if (grabbedObject && grabbedObject.IsGrabbed() && interactions.release)
        {
            grabbedObject.Drop();
            grabbedObject = null;
        }
    }


    /// <summary>
    /// Checks for any grabbable objects via raycaster, if any it outlines them.
    /// </summary>
    private void CheckForHitGrabbableObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (!hitObject.GetComponent<GrabbableObject>() && hitGrabbableObject)
                hitGrabbableObject.ToggleOutline(false);

            if (hitObject.GetComponent<GrabbableObject>())
            {
                hitGrabbableObject = hitObject.GetComponent<GrabbableObject>();
                if (!grabbedObject || (hitGrabbableObject != grabbedObject))
                {
                    hitGrabbableObject.ToggleOutline(true);
                }
            }
            else
            {
                hitGrabbableObject = null;
            }
        }
    }
}
