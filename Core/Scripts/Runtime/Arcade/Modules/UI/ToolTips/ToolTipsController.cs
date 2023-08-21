using System.Collections.Generic;
using Agate.Starcade.Scripts.Runtime.Arcade.Modules.SO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI.ToolTips
{
    public class ToolTipsController : MonoBehaviour
    {
        [SerializeField] private ToolTipsPage _toolTipsPage;
        [SerializeField] private GameObject _pageParent;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _closeButton;
        private UnityEvent OnClosePopup = new UnityEvent();

        public void Init(List<ToolTipsPageData> toolTipsPages)
        {
            // foreach (var pages in toolTipsPages)
            // {
            //     var page = Instantiate(_toolTipsPage, _pageParent.transform, true);
            //     page.SetToolTipsPage(pages);
            // }
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

        public void InitButton(UnityAction action)
        {
            if (action != null)
            {
                OnClosePopup.AddListener(action);  
            }
        }
        
        public void ClosePopUp() 
        {
            gameObject.SetActive(false); ;
            //OnButtonTriggered?.RemoveAllListeners();
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
            //Always error here 
            //if (gameObject == null) return false;
            return gameObject.activeSelf;
        }
    }
}
