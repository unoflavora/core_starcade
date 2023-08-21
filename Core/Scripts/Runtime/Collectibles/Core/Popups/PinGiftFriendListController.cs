using System;
using System.Threading.Tasks;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Core;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Core.Scripts.Runtime.Friends.UI.FriendItem;
using Agate.Starcade.Runtime.Main;
using UnityEngine;
using UnityEngine.UI;

namespace Agate.Starcade.Core.Scripts.Runtime.Collectibles.Core.Popups
{
    public class PinGiftFriendListController : MonoBehaviour
    {
        [SerializeField] private FriendItemController _friendItemPrefab;
        [SerializeField] private Transform _content;
        [SerializeField] private Button _sendGiftButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _inavailableText;

        private Action<long> _onSendGift;
        private FriendProfile _selectedFriend;

        public async void DisplayFriends(Action<FriendProfile> onSendGift, Action onClose)
        {
            _sendGiftButton.interactable = false;
            
            InitListeners(onSendGift, onClose);
            
            foreach(Transform child in _content) child.gameObject.SetActive(false);
            
            await InitFriendLists();

            if (MainSceneController.Instance.Data.FriendsData.Friends.Count == 0)
            {
                _inavailableText.SetActive(true);
                return;
            }

            
            var i = 0;
            foreach (var friend in MainSceneController.Instance.Data.FriendsData.Friends)
            {
                _inavailableText.SetActive(false);
                
                FriendItemController friendItem;

                if (i < _content.childCount)
                {
                    friendItem = _content.GetChild(i).GetComponent<FriendItemController>();
                }
                else
                {
                    friendItem = Instantiate(_friendItemPrefab, _content);
                }

                var itemSelectable = friendItem.transform.GetComponent<FriendItemSelectable>();
                itemSelectable.IsSelected = false;
                itemSelectable.RegisterOnItemClicked(HandleFriendClicked);

                friendItem.Initialize(friend.Profile, false, false);
                
                friendItem.gameObject.SetActive(true);
                i++;
            }
        }

        private static async Task InitFriendLists()
        {
            if (MainSceneController.Instance.Data.FriendsData == null)
            {
                var d = await MainSceneController.Instance.GameBackend.GetFriendList();
                MainSceneController.Instance.Data.FriendsData = d.Data;
            }
        }

        private void InitListeners(Action<FriendProfile> onSendGift, Action onClose)
        {
            _sendGiftButton.onClick.RemoveAllListeners();
            _closeButton.onClick.RemoveAllListeners();
            
            _sendGiftButton.onClick.AddListener(() =>
            {
                _closeButton.onClick.Invoke();
                onSendGift(_selectedFriend);
            });
            
            _closeButton.onClick.AddListener(() =>
            {
                gameObject.SetActive((false));
                if(onClose != null) onClose();
            });
        }

        // if friend is clicked, set friend's selected state to true
        private void HandleFriendClicked(FriendProfile friend)
        {
            foreach (var friendItem in _content.GetComponentsInChildren<FriendItemSelectable>())
            {
                var isSelected = friendItem.Id == friend.FriendCode;
                
                if (isSelected) _selectedFriend = friend;

                friendItem.IsSelected = isSelected;
                
                _sendGiftButton.interactable = true;
            }
        }

    }
}
