using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{

   public class MenuInput : MonoBehaviour
   {
      [SerializeField] private MenuManager _menuManager;
      public InputActionReference menuButton;
      
      private void Start()
      {
         menuButton.action.started += MenuButtonPressed;
      }

      private void MenuButtonPressed(InputAction.CallbackContext obj)
      {
         if (_menuManager.gameObject.activeSelf)
         {
            _menuManager.CloseMenu();
         }
         else
         {
            _menuManager.gameObject.SetActive(true);
         }
      }

      private void OnDestroy()
      {
         menuButton.action.started -= MenuButtonPressed;
      }
   }
}
