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
    /// Updates each frame, checks for user input and calls check for grab or release of the object.
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
    /// Checks via raycaster (center of the screen) if there is any object to pick up.
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
