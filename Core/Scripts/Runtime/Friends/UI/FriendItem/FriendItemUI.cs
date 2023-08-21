using System;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Lobby.Script.UserProfile.FriendsManager.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Scripts.Runtime.Friends.UI.FriendItem
{
    public class FriendItemUI : MonoBehaviour
    {
        // Fields
        [Header("UI Elements")]
        [SerializeField] private Image _profileImage;
        [SerializeField] private Image _profileFrame;
        [SerializeField] private Button _primaryButton;
        [SerializeField] private Button _secondaryButton;
        [SerializeField] private FriendStatusBox _friendStatusBox;
        [SerializeField] private GameObject _newLabel;
        
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _friendBadgeText;
        [SerializeField] private TextMeshProUGUI _friendLevelText;
        [SerializeField] private TextMeshProUGUI _friendNameText;
        [SerializeField] private TextMeshProUGUI _primaryButtonText;
        [SerializeField] private TextMeshProUGUI _secondaryButtonText;
        
        [Header("Default Images")]
        [SerializeField] private Sprite defaultProfilePicture;
        [SerializeField] private Sprite defaultFramePicture;
        
        

        // Properties
        public Button PrimaryButton => _primaryButton;
        public Button SecondaryButton => _secondaryButton;
        public FriendStatusBox FriendStatusBox => _friendStatusBox;
        
        public void SetProfileImageAndFrame(Sprite userImage, Sprite userFrame, string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
            {
                _profileImage.sprite =  userImage != null ? userImage : defaultProfilePicture;
            }
            else
            {
                Davinci.get().load(photoUrl).setCached(true).into(_profileImage).start();
            }

            _profileFrame.sprite = userFrame != null ? userFrame : defaultFramePicture;
        }

        public void SetTexts(int level, string badge, string username, FriendCategory category)
        {
            _friendBadgeText.text = badge;
            
            _friendLevelText.text = "Lv. " + level;
            
            var text = category == FriendCategory.FriendRequest 
                ? username + " wants to add you"
                : username;

            _friendNameText.enableWordWrapping = category == FriendCategory.FriendRequest;
            
            _friendNameText.overflowMode = category == FriendCategory.FriendRequest ? TextOverflowModes.Overflow : TextOverflowModes.Ellipsis;
            
            _friendNameText.SetText(text);
        }
        
        public void SetNewLabelActive(bool active)
        {
            _newLabel.SetActive(active);
        }

        public void HandleButtons(
            FriendProfile friendProfile, 
            UnityAction onUnfriendButtonClick, 
            UnityAction onSendGiftButtonClick, 
            UnityAction onAddFriendButtonClick, 
            UnityAction<bool> onFriendRequestButtonClick,
            bool displayButton
            )
        {

            _friendStatusBox.gameObject.SetActive(displayButton);
            _primaryButton.gameObject.SetActive(displayButton);
            _secondaryButton.gameObject.SetActive(displayButton);
            if(!displayButton) return;
            
            switch (friendProfile.Category)
            {
                case FriendCategory.Friend:
                    _friendBadgeText.gameObject.SetActive(true);

                    _friendStatusBox.SetAddedTextActive(false);
                    _friendStatusBox.SetRecommendationText(false);

                    _secondaryButton.gameObject.SetActive(true);
                    _secondaryButton.onClick.RemoveAllListeners();
                    _secondaryButton.onClick.AddListener(onUnfriendButtonClick);
                    _secondaryButtonText.SetText("<sprite index=4> Unfriend");

                    _primaryButton.gameObject.SetActive(true);
                    _primaryButton.onClick.RemoveAllListeners();
                    _primaryButton.onClick.AddListener(onSendGiftButtonClick);
                    _primaryButtonText.SetText("<sprite index=3> Send Gift");
                    break;
                case FriendCategory.Recommendation:
                    _friendBadgeText.gameObject.SetActive(true);

                    _friendStatusBox.SetRecommendationText(friendProfile.FriendRecommendationType == FriendRecommendationType.Facebook, "Facebook Friend");

                    _primaryButton.gameObject.SetActive(true);
                    _primaryButton.onClick.RemoveAllListeners();
                    _primaryButton.onClick.AddListener(onAddFriendButtonClick);
                    _primaryButtonText.SetText("<sprite index=2> Add Friend");

                    _secondaryButton.gameObject.SetActive(false);
                    break;
                case FriendCategory.FriendRequest:
                    _friendBadgeText.gameObject.SetActive(false);
                
                    _friendStatusBox.SetAddedTextActive(false);
                    _friendStatusBox.SetRecommendationText(false);

                    _primaryButton.gameObject.SetActive(true);
                    _primaryButtonText.SetText("<sprite index=0> Accept");
                    _primaryButton.onClick.RemoveAllListeners();
                    _primaryButton.onClick.AddListener(() => onFriendRequestButtonClick(true));

                    _secondaryButton.gameObject.SetActive(true);
                    _secondaryButtonText.SetText("<sprite index=1> Decline");
                    _secondaryButton.onClick.RemoveAllListeners();
                    _secondaryButton.onClick.AddListener(() => onFriendRequestButtonClick(false));
                    break;
            }
        }
    }
}