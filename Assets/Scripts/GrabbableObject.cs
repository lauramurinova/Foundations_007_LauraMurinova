using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    private bool isGrabbed = false;
    public Transform grabTransform;
    private Outline outline;

    private Transform originalParent;
    private CharacterController player;
    private Transform grabberObject;

    /// <summary>
    /// Initializes outline for the gameobject on start.
    /// </summary>
    private void Start()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineColor = Color.cyan;
        outline.OutlineWidth = 0f;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
    }

    /// <summary>
    /// Ensures smooth carrying of the object.
    /// </summary>
    private void Update()
    {
        if (isGrabbed)
        {
            Vector3 targetPosition = grabberObject.position + player.velocity.normalized * 0.1f;
            transform.position = Vector3.Lerp(transform.position, targetPosition, 0.05f);
        }
    }

    /// <summary>
    /// Returns the Outline Component.
    /// </summary>
    public Outline GetOutline()
    {
        return outline;
    }

    /// <summary>
    /// Toggles the outline on or off on the gameobject, based on the enable variable.
    /// </summary>
    public void ToggleOutline(bool enable)
    {
        if (enable)
        {
            outline.OutlineWidth = 4f;
        }
        else
        {
            outline.OutlineWidth = 0f;
        }
    }

    /// <summary>
    /// Grabs the object by the player.
    /// </summary>
    public void PickUp(Transform hand)
    {
        grabberObject = hand;
        originalParent = transform.parent;
        transform.parent = hand;
        GetComponent<Rigidbody>().isKinematic = true;
        transform.localEulerAngles = grabTransform.localEulerAngles;
        isGrabbed = true;
    }

    /// <summary>
    /// Releases the object grabbed currently by the player.
    /// </summary>
    public void Drop()
    {
        transform.parent = originalParent;
        GetComponent<Rigidbody>().isKinematic = false;
        isGrabbed = false;
    }

    /// <summary>
    /// Returns grabbed state.
    /// </summary>
    public bool IsGrabbed()
    {
        return isGrabbed;
    }
}
