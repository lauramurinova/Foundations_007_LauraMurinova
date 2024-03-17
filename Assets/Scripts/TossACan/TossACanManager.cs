using System;
using System.Collections.Generic;
using UnityEngine;

public class TossACanManager : MonoBehaviour
{
   [SerializeField] private List<Can> _cans;
   [SerializeField] private List<TossBall> _tossBalls;
   [SerializeField] private Animator _winAnimation;
   [SerializeField] private AudioSource _winSound;

   private bool _gameWon = false;

   private void Update()
   {
      //continuously check whether all cans has been knocked down and if player won the game
      if (CansKnockedDown() && !_gameWon)
      {
         WonGame();
      }
   }

   /// <summary>
   /// Called when user knocked down all the cans.
   /// Starts the win effects, animation of sign and play win sounds.
   /// </summary>
   private void WonGame()
   {
      _gameWon = true;
      _winAnimation.SetTrigger("Win");
      _winSound.Play();
   }

   /// <summary>
   /// Resets the game with cans and balls to initial positions.
   /// </summary>
   public void ResetGame()
   {
      _gameWon = false;
      
      foreach (Can can in _cans)
      {
         can.ResetToInitial();
      }
      
      foreach (TossBall tossBall in _tossBalls)
      {
         tossBall.ResetToInitial();
      }
   }

   /// <summary>
   /// Checks all cans states, if they have been all knocked down return true, otherwise false.
   /// </summary>
   private bool CansKnockedDown()
   {
      bool knockedDown = true;

      foreach (Can can in _cans)
      {
         if (!can.IsKnockedDown())
         {
            knockedDown = false;
         }
      }

      return knockedDown;
   }
}
