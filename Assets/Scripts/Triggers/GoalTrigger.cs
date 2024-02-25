using System.Collections;
using TMPro;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private GameObject gameInfoCanvasObject;
    [SerializeField] private TextMeshProUGUI gameInfoText;
    [SerializeField] private GameObject goalParticleSystem;

    private Coroutine _goalReachedCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        // Checks for player collision with this gameObject.
        if (other.gameObject.CompareTag("Player") && _goalReachedCoroutine == null)
        {
            _goalReachedCoroutine = StartCoroutine(GoalReachedCoroutine());
        }
    }
    
    /// <summary>
    /// Enables goal particle system and gives the player feedback, that he finished the game.
    /// </summary>
    /// 
    private IEnumerator GoalReachedCoroutine()
    {
        GameManager.GetInstance().GetPlayerController().MoveSpeed = 0f;
        goalParticleSystem.SetActive(true);
        yield return new WaitForSeconds(1f);
        gameInfoText.text = "Mission Complete";
        gameInfoCanvasObject.SetActive(true);
    }
}

