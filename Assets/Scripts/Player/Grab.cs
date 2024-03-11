using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{
    [SerializeField] private Transform _cameraPosition;
    [SerializeField] private Transform _holdPosition;
    [SerializeField] private float _grabRange = 2;
    [SerializeField] private float _throwForce = 20f;
    [SerializeField] private float _snapSpeed = 40f;

    private Rigidbody _grabbedObject;
    private bool _grabPressed = false;
    private List<Rigidbody> _inventory = new List<Rigidbody>();

    
    /// <summary>
    /// Called on user input ("E" Key).
    /// If user has a grabbed object in hands, save it to inventory.
    /// If user doesnt have a grabbed object and has items in inventory, retrieves the item from inventory.
    /// </summary>
    private void OnInventory()
    {
        if (_grabbedObject)
        {
            AddItemToInventory(_grabbedObject);
        }
        else
        {
            if (_inventory.Count > 0)
            {
                GetInventoryItem();
            }
        }
    }
    
    /// <summary>
    /// Returns the first inventory item.
    /// When the item from inventory is grabbed, it gets removed from inventory.
    /// If user would put back the item, it goes to the last position in inventory, that ensures the rotation of items.
    /// </summary>
    private void GetInventoryItem()
    {
        GrabObject(_inventory[0]);
        _grabbedObject.gameObject.SetActive(true);
        RemoveItemFromInventory(_inventory[0]);
    }
    
    /// <summary>
    /// Adds the item to inventory and hides it.
    /// </summary>
    public void AddItemToInventory(Rigidbody item)
    {
        _inventory.Add(item);
        _grabbedObject.gameObject.SetActive(false);
        _grabbedObject = null;
    }
    
    /// <summary>
    /// If item is in inventory, removes it.
    /// </summary>
    public void RemoveItemFromInventory(Rigidbody item)
    {
        if (_inventory.Contains(item))
        {
            _inventory.Remove(item);
        }
    }
    
    private void FixedUpdate()
    {
        if (_grabbedObject)
        {
            _grabbedObject.velocity = (_holdPosition.position - _grabbedObject.transform.position) * _snapSpeed;
        }
    }

    private void OnGrab()
    {
        if (_grabPressed)
        {
            _grabPressed = false;

            if (!_grabbedObject) return;

            DropGrabbedObject();
        }
        else
        {
            _grabPressed = true;

            if (Physics.Raycast(_cameraPosition.position, _cameraPosition.forward, out RaycastHit hit, _grabRange))
            {
                if (!hit.transform.gameObject.CompareTag("Grabbable")) return;

                GrabObject(hit.transform.GetComponent<Rigidbody>());

            }
        }
    }

    private void GrabObject(Rigidbody grabObject)
    {
        _grabbedObject = grabObject;
        _grabbedObject.transform.parent = _holdPosition;
        _grabbedObject.freezeRotation = true;
                
        if(!_grabbedObject.TryGetComponent(out Grabbable grabbable)) return;
        if (grabbable.grabPoint)
        {
            _grabbedObject.transform.localPosition = grabbable.grabPoint.transform.localPosition;
            _grabbedObject.transform.localRotation = grabbable.grabPoint.transform.localRotation;
        }
                
        if (_grabbedObject.TryGetComponent(out Gun gun))
        {
            gun.isGrabbed = true;
        };
    }

    private void DropGrabbedObject()
    {
        _grabbedObject.transform.parent = null;
        _grabbedObject.freezeRotation = false;

        if (_grabbedObject.TryGetComponent(out Gun gun))
        {
            gun.isGrabbed = false;
        };

        _grabbedObject = null;
    }

    private void OnThrow()
    {
        if(!_grabbedObject) return;
        
        _grabbedObject.AddForce(_cameraPosition.forward * _throwForce, ForceMode.Impulse);
        DropGrabbedObject();
    }
}
