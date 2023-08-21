using System;
using Agate.Starcade.Boot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Runtime.Lobby
{
    public class ConfirmationPopup : MonoBehaviour
    {
        
        [SerializeField] private Button _confirmButton;

        [SerializeField] private Button _cancelButton;

        [SerializeField] private ScrollRect _scrollRect;

        [SerializeField] private TextMeshProUGUI _subText;
        
        protected Action _onConfirm;

        protected Action _onCancel;

        private void Start()
        {
            _confirmButton.onClick.AddListener(() => _onConfirm.Invoke());
            
            _cancelButton.onClick.AddListener(() => _onCancel.Invoke());
        }

        public void AddListenerOnConfirmation(Action action, bool isOnce = true)
        {
            _onConfirm = action;

            if (isOnce)
            {
                _onConfirm += () => _onConfirm = () => { };
            }
        }
        
        public void AddListenerOnCancel(Action action, bool isOnce = true)
        {
            _onCancel = action;

            if (isOnce)
            {
                _onCancel += () => _onCancel = () => { };
            }
        }

        public void ResetScroll()
        {
            if(_scrollRect != null) _scrollRect.verticalNormalizedPosition = 1;
        }

        public void SetText(string text)
        {
            _subText.text = $"Your {text} Lootbox";
        }

        
    }
}
