using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Backend.Data;
using Agate.Starcade.Lobby.Script.UserProfile.FriendsManager.UI.Popups;
using Agate.Starcade.Runtime.Enums;
using Agate.Starcade.Scripts.Runtime.Collectibles.Core.Set;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Core.Popups
{
    public class FriendsPopupController : MonoBehaviour
    {
        [SerializeField] private LimitReachedPopup _limitReachedPopup;
        [SerializeField] private SearchFriendResultController _searchFriendResultPopup;
        [SerializeField] private SendGiftCollectiblePopupController _sendGiftCollectiblePopupController;
        [SerializeField] private Image _overlayBg;

        public Action<long, CurrencyTypeEnum> OnUserPurchaseLimit;
        
        public void DisplaySendGiftCollectiblePopup(string friendName, UnityAction<List<CollectibleItem>> onUserSendGift, CollectibleItem item = null)
        {
            _overlayBg.enabled = true;
            
            _sendGiftCollectiblePopupController.gameObject.SetActive(true);
            _sendGiftCollectiblePopupController.OnLimitReached = DisplayLimitReachedPopup;
            _sendGiftCollectiblePopupController.OnClosePopup = DisablePopup;
            _sendGiftCollectiblePopupController.OnSendGiftConfirmed = onUserSendGift;
            _sendGiftCollectiblePopupController.DisplayPopup(5, friendName, item);
        }

        public void OnBuySuccess()
        {
            _sendGiftCollectiblePopupController.OnBuySuccess();
        }
        
        private void DisplayLimitReachedPopup()
        {
            _sendGiftCollectiblePopupController.gameObject.SetActive(false);
            
            _limitReachedPopup.gameObject.SetActive(true);
            
            _limitReachedPopup.IsUserApprove = isUserApprove =>
            {
                if (OnUserPurchaseLimit != null && isUserApprove) OnUserPurchaseLimit(_limitReachedPopup.Price, _limitReachedPopup.CurrencyType);

                _limitReachedPopup.gameObject.SetActive(false);
                
                _sendGiftCollectiblePopupController.gameObject.SetActive(true);
            };
            
        }
        
        public void DisplaySearchFriendResultPopup(FriendsResponseData friendData, UnityAction<long> onUserAddFriend = null)
        {
            _overlayBg.enabled = true;
            
            _searchFriendResultPopup.gameObject.SetActive(true);
            
            _searchFriendResultPopup.SearchData(friendData, DisablePopup);
        }
        
        public void SetGiftDailyLimit(UserGiftDailyLimit data)
        {
            _sendGiftCollectiblePopupController.GiftDailyLimit = data.LeftAmount;
            
            _limitReachedPopup.SetPriceForAddMoreLimit(data.CurrencyType, data.CostForNextBonus);
        }

        public void DisablePopup()
        {
            _overlayBg.enabled = false;
            
            _searchFriendResultPopup.gameObject.SetActive(false);
        }
    }
}
