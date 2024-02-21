using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TextMeshProUGUI GameInfoText;
    private PlayerController player;

    private bool isPlaying = true;

    /// <summary>
    /// Creates singleton instance for easy method calls and saves variables for efficiency.
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        isPlaying = true;
    }

    /// <summary>
    /// Returns false if game has finished, otherwise true.
    /// </summary>
    public bool IsPlaying()
    {
        return isPlaying;
    }

    /// <summary>
    /// Ends game by restarting user and showing game info.
    /// </summary>
    public void EndGame()
    {
        ShowGameInfoMessage("It was your WININNG! Congrats! :)");
        player.ResetPlayer();
        isPlaying = true;
    }

    /// <summary>
    /// Checks for any trigger enter on the game object.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("PlayerCapsule") && isPlaying)
        {
            isPlaying = false;
            EndGame();
        }
    }

    /// <summary>
    /// Shows game info message with the specific text.
    /// </summary>
    public void ShowGameInfoMessage(string text)
    {
        StartCoroutine(ShowGameInfoMessageCoroutine(text));
    }

    /// <summary>
    /// Shows game message for a specific amount of time.
    /// </summary>
    private IEnumerator ShowGameInfoMessageCoroutine(string text)
    {
        GameInfoText.text = text;
        GameInfoText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        GameInfoText.gameObject.SetActive(false);
    }
}
