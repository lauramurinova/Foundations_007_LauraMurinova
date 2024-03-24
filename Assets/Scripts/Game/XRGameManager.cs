using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XRGameManager : MonoBehaviour
{
    [Header("Audio")] 
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioClip _diedMusic;
    
    [SerializeField] private GameObject _gameUICanvas;
    [SerializeField] private PlayerManager _player;
    [SerializeField] private GameObject[] _uiInteractors;
    [SerializeField] private GameObject[] _sceneInteractors;
 
    private void Start()
    {
        _player.playerDied.AddListener(GameEnd);
        _gameUICanvas.SetActive(false);
    }

    /// <summary>
    /// Called on when player dies.
    /// Sets the seen, shows reset UI, sets interactions.
    /// </summary>
    public void GameEnd()
    {
        DisableSceneInteractions();
        SetEndScene();
        StartCoroutine(ShowGameUICoroutine());
    }

    private IEnumerator ShowGameUICoroutine()
    {
        yield return new WaitForSeconds(1f);
        _player.ResetToInitial();
        EnableGameUI();
        EnableUIInteractions();
    }

    /// <summary>
    /// Plays end music and sets the environment.
    /// </summary>
    private void SetEndScene()
    {
        PlayBGM(_diedMusic);
        RenderSettings.fogColor = Color.black;
        StartCoroutine(TransitionFogDensity(0.008f, 0.7f, 2.5f));
    }
    
    /// <summary>
    /// Smoothly transitions the fog from to a given density.
    /// </summary>
    private IEnumerator TransitionFogDensity(float startDensity, float targetDensity, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, targetDensity, t);
            yield return null;
        }
        RenderSettings.fogDensity = targetDensity;
    }

    /// <summary>
    /// Enables game ui (with reset button on it).
    /// </summary>
    private void EnableGameUI()
    {
        _gameUICanvas.transform.position = _player.transform.position + _player.transform.forward + _player.transform.up;
        _gameUICanvas.SetActive(true);
    }

    /// <summary>
    /// Enables UI Interaction Points.
    /// </summary>
    private void EnableUIInteractions()
    {
        foreach (GameObject interactor in _uiInteractors)
        {
            interactor.SetActive(true);
        }
    }

    /// <summary>
    /// Disables Scene Interaction Points (teleport). 
    /// </summary>
    private void DisableSceneInteractions()
    {
        foreach (GameObject interactor in _sceneInteractors)
        {
            interactor.SetActive(false);
        }
    }

    /// <summary>
    /// Restart the current scene.
    /// </summary>
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    /// <summary>
    /// Plays background music based on given clip.
    /// </summary>
    private void PlayBGM(AudioClip newBgm)
    {
        if(_bgmSource.clip == newBgm) return;
        
        _bgmSource.clip = newBgm;
        _bgmSource.Play();
    }
}
