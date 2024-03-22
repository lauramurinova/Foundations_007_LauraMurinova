using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DoorInteractor>())
        {
            OpenDoor();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DoorInteractor>())
        {
            CloseDoor();
        }
    }

    protected void OpenDoor()
    {
        _door.SetActive(false);
    }

    protected void CloseDoor()
    {
        _door.SetActive(true);
    }
}
