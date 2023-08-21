using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.Core.Popups;
using Agate.Starcade.Core.Scripts.Runtime.Collectibles.UI.Animation;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Collectibles.Core.Popups
{
    public class CollectiblePopupController : MonoBehaviour
    {
        [SerializeField] private CollectibleItemPopup _itemPopup;
        [SerializeField] private ConvertPinPopup _convertPinPopup;
        [SerializeField] private ConfirmationPopup _confirmationPopup;
        [SerializeField] private PinGiftFriendListController _pinGiftFriendListController;
        
        [Header("Animations")]
        [SerializeField] private ConvertPinAnimation _convertPinAnimation;
        
        private Image _overlayBackground;

        public Action<string> GoToLootboxStore
        {
            set => _itemPopup.GoToStoreLootbox = value;
        }

        private void Awake()
        {
            _overlayBackground = GetComponent<Image>();
            _overlayBackground.enabled = false;

            _itemPopup.OnClosePopupClicked += ClosePopup;

            _convertPinPopup.OnClosePopupClicked += ClosePopup;
            _convertPinPopup.OnConvertPin += DisplayConfirmationPopup;

            _confirmationPopup.OnCancelClicked += () => DisplayConfirmationPopup();
            
            _confirmationPopup.OnConfirmClicked += (items) =>
            {
                CollectibleActionController.OnConvertPinConfirmed.Invoke(items);
                
            };
        }

        public void PlayConvertPinAnimation(UnityAction onConvertPinAnimationFinished)
        {
            _convertPinAnimation.Play();
            
            _convertPinAnimation.AddListenerOnce(onConvertPinAnimationFinished);
        }

        public void OpenItemPopup(CollectibleItem item, bool displayNewLabel = false, bool displayActionButton = true)
        {
            _overlayBackground.enabled = true;
            
            _itemPopup.gameObject.SetActive(true);

            _itemPopup.DisplayItem(item, displayNewLabel, displayActionButton);
        }

        public void OpenConvertPinPopup(List<CollectibleItem> items)
        {
            _overlayBackground.enabled = true;

            _convertPinPopup.gameObject.SetActive(true);
            
            _convertPinPopup.DisplayItem(items);
        }

        private void DisplayConfirmationPopup(List<CollectibleItem> items = null)
        {
            bool displayConfirmationPopup = items != null;
            
            Debug.Log(displayConfirmationPopup);
            
            _confirmationPopup.gameObject.SetActive(displayConfirmationPopup);
            
            _convertPinPopup.gameObject.SetActive(!displayConfirmationPopup);
            
            if (!displayConfirmationPopup) return;
            
            _confirmationPopup.DisplayPopup(items);
        }
        
        public void DisplayFriendListPopup(Action<FriendProfile> onSendPin)
        {
            CloseAllPopup();
            _overlayBackground.enabled = true;
            _pinGiftFriendListController.gameObject.SetActive(true);
            _pinGiftFriendListController.DisplayFriends(onSendPin, ClosePopup);
        }
        
        private void ClosePopup()
        {
            _overlayBackground.enabled = false;
        }

        public void CloseAllPopup()
        {
            _confirmationPopup.gameObject.SetActive(false);
            _itemPopup.gameObject.SetActive(false);
            _convertPinPopup.gameObject.SetActive(false);
            _pinGiftFriendListController.gameObject.SetActive(false);
            ClosePopup();
        }

        public void EnablePopupBackground(bool enable)
        {
            _overlayBackground.enabled = enable;
        }
    }
}
