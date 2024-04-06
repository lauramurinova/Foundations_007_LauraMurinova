using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _windows;
        [SerializeField] private TMP_Text _dominantHandText;

        private void Awake()
        {
            Handed handadness = (Handed)PlayerPrefs.GetInt("Handedness");
            if (handadness.Equals(Handed.Left))
            {
                _dominantHandText.text = "Left";
            }
            else
            {
                _dominantHandText.text = "Right";
            }
        }

        public void OpenWindow(int index)
        {
            CloseAllWindows();
            _windows[index].SetActive(true);
        }

        public void CloseAllWindows()
        {
            foreach (var win in _windows)
            {
                win.SetActive(false);
            }
        }

        private void DeactivateMenu()
        {
            gameObject.SetActive(false);
        }

        public void CloseMenu()
        {
            CloseAllWindows();
            Invoke(nameof(DeactivateMenu), 0.1f);
        }
    }
}
