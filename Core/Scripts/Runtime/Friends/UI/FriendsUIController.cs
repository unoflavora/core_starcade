using System;
using System.Collections.Generic;
using Agate.Starcade.Core.Runtime.Game;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Backend.Data;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Core;
using Agate.Starcade.Core.Scripts.Runtime.Friends.Data;
using Agate.Starcade.Core.Runtime.Lobby.User_Profile.Friends_Manager.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Agate.Starcade.Scripts.Runtime.Lobby.Friends_Manager.Core
{
    public class FriendsUIController : MonoBehaviour
    {
        [Header("Friends UI Modules")]
        [SerializeField] private TextMeshProUGUI _userId;
        [SerializeField] private TextMeshProUGUI _friendsCount;
        [SerializeField] private List<FriendsPanelController> _friendsListPanels;

        [Header("Friends Interactables")]
        [SerializeField] private Button _searchButton;
        [SerializeField] private TMP_InputField _searchInputField;
        [SerializeField] private Button _refreshButton;

        [Header("Notifications")]
        [SerializeField] private NotificationBadge _friendRequestNotification;
        [SerializeField] private NotificationBadge _friendListNotification;
        [SerializeField] private NotificationBadge _friendsTabNotification;
        
        [Header("Friend List Item Prefab")]
        public GameObject friendListItemPrefab;

        // DATA
        private List<FriendProfile> friendList;
        private List<FriendItemController> friendListItems;
        
        // EVENT LISTENERS
        public UnityAction<long> OnSearchFriend;
        public UnityAction OnRefreshClicked;

        private int _newFriendsCount;
        private int _newFriendRequestsCount;
        public bool _friendsInit;
        
        private void Start()
        {
            _searchButton.onClick.AddListener(() =>
            {
                OnSearchFriend?.Invoke(long.Parse(_searchInputField.text));
                
                _searchInputField.SetTextWithoutNotify(string.Empty);
            });
            
            _refreshButton.onClick.AddListener(OnRefreshButtonClicked);
            
            FriendsEventBus.Instance.Subscribe(FriendsEvent.OnFriendStatusChanged, SetUserUIFromStatus);
        }
        private void Update()
        {
            if (!_friendsInit) return;
            
            if (_newFriendsCount == 0 && _newFriendRequestsCount == 0) _friendsTabNotification.DisableBadge();
            
            else _friendsTabNotification.EnableBadgeWithCounter(_newFriendsCount + _newFriendRequestsCount);
        }
        
        public void Init(long userId)
        {
            SetUserID(userId);
            
            _friendsInit = false;

            foreach (var panel in _friendsListPanels)
            {
                var panelActive = panel.name == "Friend List";

                panel.gameObject.SetActive(panelActive);
            }
        }
        private void OnRefreshButtonClicked()
        {
            OnRefreshClicked?.Invoke();
        }
        public void SetActivePanel(string panelName)
        {
            foreach (var panel in _friendsListPanels)
            {
                var panelActive = panelName == panel.name;
                
                panel.gameObject.SetActive(panelActive);

                if (!panelActive) continue;
                
                switch (panel.name)
                {
                    case "Friend List":
                        _friendListNotification.DisableBadge();
                        _newFriendsCount = 0;
                        break;
                    case "Request":
                        _friendRequestNotification.DisableBadge();
                        _newFriendRequestsCount = 0;
                        break;
                }

                foreach (Transform child in panel.friendItems)
                {
                    var friendItem = child.GetComponent<FriendItemController>();

                    if (friendItem != null)
                    {
                        if (panel.name == "Friend List")
                            FriendsPrefManager.SaveFriendToPlayerPrefs(friendItem.FriendProfile.FriendCode);
                        if (panel.name == "Request")
                            FriendsPrefManager.SaveFriendRequestToPlayerPrefs(friendItem.FriendProfile.FriendCode);
                    }
                }
            }
        }
        public void SetFriendListData(List<FriendProfile> friendDatas)
        {
            friendList = friendDatas;
            _newFriendsCount = 0;
            _newFriendRequestsCount = 0;
            _friendListNotification.DisableBadge();
            _friendRequestNotification.DisableBadge();

            // Initialize friend list items
            for (int i = 0; i < friendList.Count; i++)
            {
                var data = friendList[i];
                
                NotifyUserOfNewItem(data.IsNew ? data : null);

                FriendItemController friendListItemController = GetFriendListItem(i);
                
                friendListItemController.Initialize(data);
            }
            
            // Set inactive any extra friend list items
            for (int i = friendList.Count; i < friendListItems.Count; i++)
            {
                friendListItems[i].gameObject.SetActive(false);
            }

            _friendsInit = true;

        }
        private void NotifyUserOfNewItem(FriendProfile profile)
        {
            if (profile == null) return;
            
            if (profile.Category == FriendCategory.Friend)
            {
                _newFriendsCount++;
                _friendListNotification.EnableBadge();
            }
            
            else if (profile.Category == FriendCategory.FriendRequest)
            {
                _friendRequestNotification.EnableBadge();
                _newFriendRequestsCount++;
            }
        }
        private void SetUserID(long id)
        {
            _userId.text = "Your ID: " + id + " <sprite=0>";
        }
        public void SetFriendCount(int count, int maxCount)
        {
            _friendsCount.text = count + "/" + maxCount;
        }
        private void SetUserUIFromStatus(object data)
        {
            var eventData = (FriendStatusChangedEventData) data;
            
            var friendItem = friendListItems.Find(item => item.FriendProfile.FriendCode == eventData.FriendCode);

            if (friendItem == null) return;
            
            friendItem.FriendProfile.IsNew = false;

            switch (eventData.Status)
            {
                case FriendRequestStatus.NotFriend:
                    friendItem.transform.parent = transform;
                    friendItem.gameObject.SetActive(false);
                    break;
                case FriendRequestStatus.Approved:
                    NotifyUserOfNewItem(friendItem.FriendProfile);
                    friendItem.transform.parent = _friendsListPanels[0].transform;
                    friendItem.FriendProfile.Category = FriendCategory.Friend;
                    break;
            }
            
        }
        private FriendItemController GetFriendListItem(int index)
        {
            FriendItemController friendListItemController;
            if (friendListItems == null) friendListItems = new List<FriendItemController>();
            
            var parent = GetPanel(index);

            if (index >= friendListItems.Count)
            {
                friendListItemController = InstantiateFriend(parent);
            }
            else
            {
                friendListItemController = friendListItems[index];
                friendListItemController.gameObject.SetActive(true);
                friendListItemController.transform.SetParent(parent);
            }
            
            return friendListItemController;
        }
        private Transform GetPanel(int index)
        {
            Transform parent = friendList[index].Category switch
            {
                FriendCategory.Friend => _friendsListPanels[0].friendItems,
                FriendCategory.Recommendation => _friendsListPanels[1].friendItems,
                FriendCategory.FriendRequest => _friendsListPanels[2].friendItems,
                _ => transform
            };
            return parent;
        }
        private FriendItemController InstantiateFriend(Transform parent)
        {
            FriendItemController friendListItemController;

            GameObject friendListItemObject = Instantiate(friendListItemPrefab, parent);
            friendListItemController = friendListItemObject.GetComponent<FriendItemController>();
            friendListItems.Add(friendListItemController);
            return friendListItemController;
        }
        // Clean up pooled objects when the scene is unloaded
        private void OnDestroy()
        {
            if (friendListItems == null) return;
            
            foreach (FriendItemController friendListItem in friendListItems)
            {
                Destroy(friendListItem.gameObject);
            }

            friendListItems.Clear();
        }
    }
}
