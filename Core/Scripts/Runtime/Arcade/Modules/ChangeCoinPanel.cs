using System.Collections.Generic;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Arcade.Modules.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Starcade.Core.Scripts.Runtime.Arcade.Modules
{
    public class ChangeCoinPanel : PopupController
    {
        [SerializeField] private List<Sprite> _coinIcon;
        [SerializeField] private List<string> _descriptionList;

        public UnityEvent OnClosePopup { get; set; } = new UnityEvent();
		public UnityEvent OnChangeModeSelected { get; set; } = new UnityEvent();
        public List<Sprite> coinIcon
        {
            get => _coinIcon;
            set => _coinIcon = value;
        }

        public List<string> descriptionList
        { 
            get => _descriptionList;
            set => _descriptionList = value;
        }

        private void Start()
        {
			AcceptButton.onClick.AddListener(delegate { OnChangeModeSelected.Invoke(); });
			DeclineButton.onClick.AddListener(Close);
		}

        public void DisplayChangeCoin(GameModeEnum mode)
        {
            //if (action != null)
            //{
            //    OnClosePopup.AddListener(action);  
            //}
            
            if (mode == GameModeEnum.Gold)
            {
                DescriptionText.text = descriptionList[0];
                PopupImages.sprite = coinIcon[0];
            }
            else
            {
                DescriptionText.text = descriptionList[1];
                PopupImages.sprite = coinIcon[1];
            }

            gameObject.SetActive(true);
            
        }

        public void Close()
        {
			gameObject.SetActive(false);
			OnClosePopup.Invoke();
			OnClosePopup?.RemoveAllListeners();
		}
    }
}
