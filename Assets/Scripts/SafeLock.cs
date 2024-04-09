using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class SafeLockLevel
{
    public int value;
}

public class SafeLock : MonoBehaviour
{
    [SerializeField] private TextMeshPro _safeText;
    [SerializeField] private Transform _rotator;
    [SerializeField] private SafeLockLevel[] _levels;
    [SerializeField] private AudioSource _levelUnlockAudio;
    [SerializeField] private AudioSource _safeUnlockAudio;
    [SerializeField] private SafeDoor _safeDoor;

    private int _currentlyUnlockingLevel = 0;
    private bool _unlocked = false;
    private int _currentValue = 0;
    private Vector3 _lastLockRotation = Vector3.zero;
    private bool _changingLevel = false;
    private string _state = "";

    private void Start()
    {
        Assert.IsNotNull(_rotator, "You have not assigned a rotator to the safe lock of object " + name);
        _state = (_currentlyUnlockingLevel + 1).ToString();
    }

    void Update()
    {
        if(_changingLevel) return;
        
        CheckForRightCombination(_currentlyUnlockingLevel);
        CalculateLockValue();
        UpdateDisplay();
    }

    /// <summary>
    /// Calculates the value of the safe lock thats currently set by rotation.
    /// </summary>
    private void CalculateLockValue()
    {
        if (_lastLockRotation.z - _rotator.transform.rotation.eulerAngles.z > 20)
        {
            _currentValue++;
            _lastLockRotation = _rotator.transform.rotation.eulerAngles;
        }
        else if(_lastLockRotation.z - _rotator.transform.rotation.eulerAngles.z < -20)
        {
            _currentValue--;
            _lastLockRotation = _rotator.transform.rotation.eulerAngles;
        }
    }

    /// <summary>
    /// Updates the safe display values - to show user a feedback.
    /// </summary>
    private void UpdateDisplay()
    {
        _safeText.text = "Value: " + _currentValue + "\n\nCode: " + _levels[0].value +", " + _levels[1].value
                         +", " + _levels[2].value +"\nState: "+ _state;
    }

    /// <summary>
    /// Checks whether the safe current value is the one required by the current level.
    /// </summary>
    private void CheckForRightCombination(int level)
    {
        if(_unlocked) return;
        
        if (_currentValue.Equals(_levels[level].value))
        {
            if (_currentlyUnlockingLevel == _levels.Length-1)
            {
                UnlockSafe();
            }
            else
            {
                _changingLevel = true;
                StartCoroutine(ChangeLevel());
            }
        }
    }

    /// <summary>
    /// Changes the level on the safe - upgrades to next one.
    /// Called when user unlocked the previous level.
    /// </summary>
    private IEnumerator ChangeLevel()
    {
        _levelUnlockAudio.Play();
        yield return new WaitForSeconds(0.1f);
        _currentlyUnlockingLevel++;
        _state = (_currentlyUnlockingLevel + 1).ToString();
        _changingLevel = false;
    }

    /// <summary>
    /// Unlocks the safe and sets the door open.
    /// </summary>
    private void UnlockSafe()
    {
        _safeUnlockAudio.Play();
        _safeDoor.Open();
        _unlocked = true;
        _state = "Unlocked";
        _safeText.color = Color.green;
    }
}
