using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 initialLocalPosition;
    Vector3 initialGlobalPosition;
    private float levelBottomBoundary = -20f;
    private FirstPersonController firstPersonController;

    /// <summary>
    /// Called on start, saves variables for efficiency.
    /// </summary>
    private void Start()
    {
        initialLocalPosition = transform.localPosition;
        initialGlobalPosition = transform.position;
        firstPersonController = GetComponent<FirstPersonController>();
    }

    /// <summary>
    /// Updates each frame, checks wether the player had fallen, if so calls for reset.
    /// </summary>
    private void Update()
    {
        if (transform.position.y < levelBottomBoundary)
        {
            GameManager.instance.ShowGameInfoMessage("Try again! Good luck! :)");
            ResetPlayer();
        }
    }

    /// <summary>
    /// Resets the player to the initial position.
    /// </summary>
    public void ResetPlayer()
    {
        StartCoroutine(ResetPlayerCoroutine());
    }

    /// <summary>
    /// Resets the player to the initial position after specific time.
    /// </summary>
    private IEnumerator ResetPlayerCoroutine()
    {
        firstPersonController.enabled = false;
        transform.localPosition = initialLocalPosition;
        transform.position = initialGlobalPosition;
        yield return new WaitForSeconds(1f);
        firstPersonController.enabled = true;
    }
}
