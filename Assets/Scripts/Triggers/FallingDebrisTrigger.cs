using System.Collections.Generic;
using UnityEngine;

public class FallingDebrisTrigger : MonoBehaviour
{
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private float triggerTimeInSeconds = 3f;

    private float _timeCounter = 0f;
    private bool _hasFallen = false;
    private List<GameObject> _debrisGameObjects = new List<GameObject>(); 
    
    private void OnTriggerStay(Collider other)
    {
        // Checks if player is within the trigger for more than 3 seconds, if so calls for Debris fall.
        if (other.CompareTag("Player"))
        {
            _timeCounter += Time.deltaTime;
            if (_timeCounter > triggerTimeInSeconds && !_hasFallen)
            {
                FallDebris();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // If player leaves the trigger area, reset values - enabling a new spawn.
        _timeCounter = 0f;
        _hasFallen = false;
    }

    /// <summary>
    /// Creates a Debris instance, makes it fall in front of the player.
    /// Instance is saved in a list for later access.
    /// </summary>
    private void FallDebris()
    {
        _hasFallen = true;
        var playerTransform = GameManager.GetInstance().GetPlayer().transform;
        _debrisGameObjects.Add(Instantiate(
            debrisPrefab,
            playerTransform.position + playerTransform.forward * 4f + Vector3.up * 4f,
            playerTransform.rotation,
            transform.parent));
    }
}
