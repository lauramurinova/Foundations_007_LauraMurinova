using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    /// <summary>
    /// Checks for player collision with the object.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            Debug.Log("Player Enters Trigger Volume");
     }
    }
}