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
      if (CansKnockedDown() && !_gameWon)
      {
         WonGame();
      }
   }

   private void WonGame()
   {
      _gameWon = true;
      _winAnimation.SetTrigger("Win");
      _winSound.Play();
   }

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
