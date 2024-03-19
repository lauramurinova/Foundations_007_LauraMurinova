using System;
using TMPro;
using UnityEngine;

public class TargetHuntManager : CarnivalActivity
{
    [SerializeField] private MovingTarget[] _movingTargets;
    [SerializeField] private TextMeshPro _highestScoreText;
    [SerializeField] private TextMeshPro _currentScoreText;
    
    private bool _gameWon = false;
    private int _currentScore = 0;
    private int _highestScore = 0;

    private void Start()
    {
        UpdateScoreText();
        SetTargetKnockedDownListeners();
    }

    /// <summary>
    /// Sets the listener for each moving target, to know when a target has been knocked down.
    /// </summary>
    private void SetTargetKnockedDownListeners()
    {
        foreach (MovingTarget movingTarget in _movingTargets)
        {
            movingTarget.knockedDownEvent.AddListener(score =>
            {
                TargetKnockedDown(score);
            });
        }
    }

    /// <summary>
    /// Sets game state, score and text according to the target that has been knocked down.
    /// </summary>
    private void TargetKnockedDown(int score)
    {
        UpdateScore(score);
        CheckGameState();
    }

    /// <summary>
    /// Checks whether the game has been won.
    /// </summary>
    private void CheckGameState()
    {
        if (TargetsKnockedDown() && !_gameWon)
        {
            WonGame();
        }
    }

    /// <summary>
    /// Updates player's current score, highest score if surpassed.
    /// </summary>
    public void UpdateScore(int scoreToAdd)
    {
        _currentScore += scoreToAdd;
        if (_currentScore > _highestScore)
        {
            _highestScore = _currentScore;
        }

        UpdateScoreText();
    }

    /// <summary>
    /// Sets player's text in the game according to set scores.
    /// </summary>
    private void UpdateScoreText()
    {
        _highestScoreText.text = "Highest Score:\n"+ _highestScore;
        _currentScoreText.text = "Your Score:\n"+_currentScore;
    }

    /// <summary>
    /// Checks all targets states, if they have been all knocked down return true, otherwise false.
    /// </summary>
    private bool TargetsKnockedDown()
    {
        bool knockedDown = true;

        foreach (MovingTarget movingTarget in _movingTargets)
        {
            if (!movingTarget.IsKnockedDown())
            {
                knockedDown = false;
            }
        }

        return knockedDown;
    }
    
    /// <summary>
    /// Resets the game to the initial state.
    /// </summary>
    public override void ResetGame()
    {
        foreach (MovingTarget movingTarget in _movingTargets)
        {
            movingTarget.ResetToInitial();
        }
        _gameWon = false;
        _currentScore = 0;
        UpdateScoreText();
    }
    
    /// <summary>
    /// Called when user knocked down all the clowns.
    /// </summary>
    protected override void WonGame()
    {
        base.WonGame();
        _gameWon = true;
    }
}
