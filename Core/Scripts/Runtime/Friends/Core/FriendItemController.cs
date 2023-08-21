using System;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Core.Scripts.Runtime.Friends.UI.FriendItem;
using Agate.Starcade.Core.Runtime.Analytics.Handlers;
using Agate.Starcade.Runtime.Main;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.Core
{
    public class FriendItemController : MonoBehaviour
    {
        // UI
        [SerializeField] private FriendItemUI _friendItemUI;

        //Data
        public FriendProfile FriendProfile { get; private set; }
        
        // State
        private bool _isInitialized;

        // Methods
        public void Initialize(FriendProfile friendProfile, bool displayButtons = true, bool displayNewLabelIfNew = true)
        {
            FriendProfile = friendProfile;
            
            gameObject.name = friendProfile.Username;

            SetUIButtons(friendProfile, displayButtons);

            var playerAvatar = friendProfile.UsedAvatar != null
                ? MainSceneController.Instance.AssetLibrary.GetSpriteAsset(friendProfile.UsedAvatar)
                : null;

            var playerFrame = friendProfile.UsedFrame != null
                ? MainSceneController.Instance.AssetLibrary.GetSpriteAsset(friendProfile.UsedFrame)
                : null;
            
            _friendItemUI.SetProfileImageAndFrame(playerAvatar, playerFrame, friendProfile.PhotoUrl);
            
            _friendItemUI.SetTexts(level: friendProfile.Level, badge: "", friendProfile.Username, friendProfile.Category);

            _friendItemUI.SetNewLabelActive(friendProfile.IsNew && displayNewLabelIfNew);

            if (_isInitialized) return;
            
            _isInitialized = true;
            
            FriendsEventBus.Instance.Subscribe(FriendsEvent.OnFriendStatusChanged, OnStatusChanged);
        }

        private void SetUIButtons(FriendProfile friendProfile, bool displayButtons)
        {
            _friendItemUI.HandleButtons(
                friendProfile,  
                OnUnfriendButtonClick, 
                OnSendGiftButtonClick, 
                OnAddFriendButtonClick,
                OnFriendRequestButtonClick,
                displayButtons
                );
        }

        #region UI Events
        private void OnUnfriendButtonClick()
        {
            FriendsEventBus.Instance.Publish(FriendsEvent.OnUnfriendFriend, new CurrentFriendInteractionData(FriendProfile.FriendCode, FriendProfile.Username));
        }
        
        private void OnSendGiftButtonClick()
        {
            MainSceneController.Instance.Analytic.LogEvent(FriendAnalyticEventHandler.ANALYTIC_KEY.CLICK_GIVE_FRIEND_COLLECTIBLE_PIN_EVENT);

            FriendsEventBus.Instance.Publish(FriendsEvent.OnSendGiftToFriend, new CurrentFriendInteractionData(FriendProfile.FriendCode, FriendProfile.Username));
        }

        private void OnAddFriendButtonClick()
        {
            FriendsEventBus.Instance.Publish(FriendsEvent.OnSendFriendRequest, FriendProfile.FriendCode);
        }
        
        private void OnFriendRequestButtonClick(bool isAccepted)
        {
            FriendsEventBus.Instance.Publish(FriendsEvent.OnFriendRequestAction, new FriendRequestEventData(FriendProfile.FriendCode, isAccepted));
        }
        
        # endregion
        
        #region Social

        private void OnStatusChanged(object statusData)
        {
            var eventData = (FriendStatusChangedEventData) statusData;
            
            if(eventData.FriendCode != FriendProfile.FriendCode) return;
            
            switch (eventData.Status)
            {
                case FriendRequestStatus.PendingApproval:
                    OnAddFriendSuccess();
                    break;
                case FriendRequestStatus.Approved:
                    FriendProfile.Category = FriendCategory.Friend;
                    SetUIButtons(FriendProfile, true);
                    break;
                case FriendRequestStatus.NotFriend:
                    FriendProfile.Category = FriendCategory.Recommendation;
                    SetUIButtons(FriendProfile, true);
                    break;
            }
        }

        private void OnAddFriendSuccess()
        {
            _friendItemUI.PrimaryButton.gameObject.SetActive(false);
            _friendItemUI.SecondaryButton.gameObject.SetActive(false);
            _friendItemUI.FriendStatusBox.SetAddedTextActive(true);
        }

        private void OnDestroy()
        {
            FriendsEventBus.Instance.Unsubscribe(FriendsEvent.OnFriendStatusChanged, OnStatusChanged);
        }

        #endregion
    }
}
