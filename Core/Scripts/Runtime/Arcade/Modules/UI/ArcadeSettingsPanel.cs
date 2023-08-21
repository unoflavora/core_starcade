using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Agate.Starcade.Scripts.Runtime.Game;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI
{
    public class ArcadeSettingsPanel : SettingsController
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _closeButton;
        private UnityEvent OnClosePopup { get; set; } = new UnityEvent();
        
        public void Init(UnityAction action)
        {
            InitSettings();

            if (action != null)
            {
                OnClosePopup.AddListener(action);  
            }
        }

        private void OnEnable()
        {
            _backButton.onClick.AddListener(BackAction);
            _closeButton.onClick.AddListener(ClosePopUp);
        }

        private void OnDisable()
        {
            _backButton.onClick.AddListener(() => _backButton.onClick.RemoveListener(BackAction));
            _closeButton.onClick.AddListener(() => _closeButton.onClick.RemoveListener(ClosePopUp));
        }

        private void ClosePopUp() 
        {
            gameObject.SetActive(false);
        }
        
        private void BackAction() 
        {
            gameObject.SetActive(false);
            OnClosePopup.Invoke();
        }

        public void DisplayPanel()
        {
            gameObject.SetActive(true);
        }

        public bool GetIsActive()
        {
            return gameObject.activeSelf;
        }
        
    }
}
