using System;
using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameEventManager : MonoBehaviour
{
    public enum GameMode
    {
        FP, VR
    }
    
    [Header("Accessibility")] public Handed handedness;

    public GameMode gameMode;
    
    [Header("UI")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _failedPanel;
    [SerializeField] private GameObject _successPanel;
    [SerializeField] private float _canvasFadeTime = 2f;
    [SerializeField] private Material _skyboxMaterial;

    [Header("Audio")] 
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioClip _caughtMusic;
    [SerializeField] private AudioClip _successMusic;

    // private PlayerInput _playerInput;
    // private FirstPersonController _fpController;
    private bool _isFadingIn = false;
    private float _fadeLevel = 0f;
    private bool _isGoalReached = false;

    private Color _initialSkyboxColor;
    private float _initialSkyboxExposure;
    private float _initialAthmosphereThickness;

    private void Awake()
    {
        handedness = (Handed)PlayerPrefs.GetInt("Handedness");
    }

    void Start()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            enemy.onInvestigate.AddListener(EnemyInvestigating);
            enemy.onPlayerFound.AddListener(PlayerFound);
            enemy.onReturnToPatrol.AddListener(EnemyReturnToPatrol);
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            // _playerInput = player.GetComponent<PlayerInput>();
            // _fpController = player.GetComponent<FirstPersonController>();
        }
        else
        {
            Debug.LogWarning("There is no player in the scene.");
        }

        _canvasGroup.alpha = 0f;
        _failedPanel.SetActive(false);
        _successPanel.SetActive(false);
        
        ResetShaderValues();
        _initialAthmosphereThickness = _skyboxMaterial.GetFloat("_AtmosphereThickness");
        _initialSkyboxExposure = _skyboxMaterial.GetFloat("_Exposure");
        _initialSkyboxColor = _skyboxMaterial.GetColor("_SkyTint");
    }

    private void Update()
    {
        if (_isFadingIn)
        {
            if (_fadeLevel < 1f)
            {
                _fadeLevel += Time.deltaTime / _canvasFadeTime;
            }
        }
        else
        {
            if (_fadeLevel < 0f)
            {
                _fadeLevel -= Time.deltaTime / _canvasFadeTime;
            }
        }
        _canvasGroup.alpha = _fadeLevel;
    }

    public void RestartScene()
    {
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        ResetShaderValues();
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void GoalReached()
    {
        _isFadingIn = true;
        _isGoalReached = true;
        
        _successPanel.SetActive(true);
        
        // DeactivateInput();
        PlayBGM(_successMusic);
    }

    private void EnemyInvestigating()
    {
        
    }
    
    private void PlayerFound(Transform enemyThatFoundPlayer)
    {
        if (_isGoalReached) return;

        _failedPanel.SetActive(true);
        if (gameMode == GameMode.FP)
        {
            _isFadingIn = true;
            // _fpController.CinemachineCameraTarget.transform.LookAt(enemyThatFoundPlayer);
        }
        else
        {
            StartCoroutine(GameOverFadeOut(1));
        }
        
        // DeactivateInput();
        PlayBGM(_caughtMusic);
    }

    private IEnumerator GameOverFadeOut(float _startDelay = 0f)
    {
        yield return new WaitForSeconds(_startDelay);
        
        Time.timeScale = 0;
        
        float fade = 0;

        while (fade < 1)
        {
            fade += Time.unscaledDeltaTime / _canvasFadeTime;
            Shader.SetGlobalFloat("_AllWhite", fade);
            _skyboxMaterial.SetFloat("_AtmosphereThickness", Mathf.Lerp(_initialAthmosphereThickness, 0.7f, fade));
            _skyboxMaterial.SetFloat("_Exposure",Mathf.Lerp(_initialSkyboxExposure, 8, fade));
            _skyboxMaterial.SetColor("_SkyTint", Color.Lerp(_initialSkyboxColor, Color.white, fade));
            
            yield return null;
        }

        yield return new WaitForSecondsRealtime(2f);
        RestartScene();
    }

    private void OnDestroy()
    {
        ResetShaderValues();
    }

    private void ResetShaderValues()
    {
        Shader.SetGlobalFloat("_AllWhite", 0);
        _skyboxMaterial.SetFloat("_AtmosphereThickness", _initialAthmosphereThickness);
        _skyboxMaterial.SetFloat("_Exposure", _initialSkyboxExposure);
        _skyboxMaterial.SetColor("_SkyTint", _initialSkyboxColor);
    }

    private void DeactivateInput()
    {
        // _playerInput.DeactivateInput();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void PlayBGM(AudioClip newBgm)
    {
        if(_bgmSource.clip == newBgm) return;
        
        _bgmSource.clip = newBgm;
        _bgmSource.Play();
    }

    private void EnemyReturnToPatrol()
    {
        
    }

    public void ToggleDominantHand()
    {
        if (handedness.Equals(Handed.Right))
        {
            handedness = Handed.Left;
        }
        else
        {
            handedness = Handed.Right;
        }
        
        PlayerPrefs.SetInt("Handedness", (int)handedness);
        PlayerPrefs.Save();
        
        RestartScene();
    }
}
