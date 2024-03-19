using UnityEngine;

public abstract class CarnivalActivity : MonoBehaviour
{
   [SerializeField] protected Animator _winAnimation;
   [SerializeField] protected AudioSource _winSound;
   
   public abstract void ResetGame();

   /// <summary>
   /// Called when user won a game.
   /// Starts the win effects, animation of sign and play win sounds.
   /// </summary>
   protected virtual void WonGame()
   {
      _winAnimation.SetTrigger("Win");
      _winSound.Play();
   }
}
