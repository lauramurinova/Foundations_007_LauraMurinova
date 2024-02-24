using StarterAssets;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    [SerializeField] private GameObject player;

    private FirstPersonController _firstPersonController;

    private void Awake()
    {
        // creates the singleton of the manager
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    
    private void Start()
    {
        // saving reference for efficiency
        _firstPersonController = player.GetComponent<FirstPersonController>();
    }

    /// <summary>
    /// Returns player of the game.
    /// </summary>
    public GameObject GetPlayer()
    {
        return player;
    }

    /// <summary>
    /// Returns player's controller script.
    /// </summary>
    public FirstPersonController GetPlayerController()
    {
        return _firstPersonController;
    }

    /// <summary>
    /// Returns singleton instance of the manager - enables access to the manager.
    /// </summary>
    public static GameManager GetInstance()
    {
        return _instance;
    }
}
